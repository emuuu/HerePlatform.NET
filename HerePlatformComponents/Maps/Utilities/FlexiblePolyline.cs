using System;
using System.Collections.Generic;
using System.Text;

namespace HerePlatformComponents.Maps.Utilities;

/// <summary>
/// Pure C# codec for HERE Flexible Polyline encoding.
/// See: https://github.com/heremaps/flexible-polyline
/// </summary>
public static class FlexiblePolyline
{
    private const int DefaultPrecision = 5;

    private static readonly char[] EncodingTable =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_".ToCharArray();

    // Indexed by (ASCII code - 45). Values 0-63 are valid; -1 = invalid.
    private static readonly int[] DecodingTable =
    {
        62, -1, -1, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1,
         0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
        20, 21, 22, 23, 24, 25, -1, -1, -1, -1, 63, -1, 26, 27, 28, 29, 30, 31, 32, 33,
        34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51
    };

    /// <summary>
    /// Decodes a flexible polyline string into a list of coordinates.
    /// </summary>
    public static List<LatLngLiteral> Decode(string encoded)
    {
        var result = new List<LatLngLiteral>();
        if (string.IsNullOrEmpty(encoded))
            return result;

        var index = 0;

        // Decode header
        long headerVersion = DecodeUnsignedVarint(encoded, ref index);
        long headerContent = DecodeUnsignedVarint(encoded, ref index);

        var precision = (int)(headerContent & 0x0F);
        var thirdDim = (int)((headerContent >> 4) & 0x07);
        var thirdDimPrecision = (int)((headerContent >> 7) & 0x0F);

        var multiplier = Math.Pow(10, precision);

        long lat = 0, lng = 0, z = 0;

        while (index < encoded.Length)
        {
            long deltaLat = DecodeSignedVarint(encoded, ref index);
            if (index >= encoded.Length) break;
            long deltaLng = DecodeSignedVarint(encoded, ref index);

            lat += deltaLat;
            lng += deltaLng;

            if (thirdDim > 0 && index < encoded.Length)
            {
                long deltaZ = DecodeSignedVarint(encoded, ref index);
                z += deltaZ;
            }

            result.Add(new LatLngLiteral(lat / multiplier, lng / multiplier));
        }

        return result;
    }

    /// <summary>
    /// Encodes a list of coordinates into a flexible polyline string.
    /// </summary>
    public static string Encode(IEnumerable<LatLngLiteral> coordinates, int precision = DefaultPrecision)
    {
        var sb = new StringBuilder();
        var multiplier = Math.Pow(10, precision);

        // Header: version=1, precision in lower 4 bits, no 3rd dim
        long headerContent = precision;
        EncodeUnsignedVarint(1, sb); // version
        EncodeUnsignedVarint(headerContent, sb); // content header

        long prevLat = 0, prevLng = 0;

        foreach (var coord in coordinates)
        {
            var lat = (long)Math.Round(coord.Lat * multiplier);
            var lng = (long)Math.Round(coord.Lng * multiplier);

            EncodeSignedVarint(lat - prevLat, sb);
            EncodeSignedVarint(lng - prevLng, sb);

            prevLat = lat;
            prevLng = lng;
        }

        return sb.ToString();
    }

    private static long DecodeUnsignedVarint(string encoded, ref int index)
    {
        long result = 0;
        var shift = 0;

        while (index < encoded.Length)
        {
            var c = DecodeChar(encoded[index++]);
            result |= (long)(c & 0x1F) << shift;
            if ((c & 0x20) == 0)
                break;
            shift += 5;
        }

        return result;
    }

    private static long DecodeSignedVarint(string encoded, ref int index)
    {
        long unsigned = DecodeUnsignedVarint(encoded, ref index);
        return (unsigned & 1) != 0 ? ~(unsigned >> 1) : (unsigned >> 1);
    }

    private static void EncodeUnsignedVarint(long value, StringBuilder sb)
    {
        while (value > 0x1F)
        {
            sb.Append(EncodingTable[(int)((value & 0x1F) | 0x20)]);
            value >>= 5;
        }
        sb.Append(EncodingTable[(int)(value & 0x1F)]);
    }

    private static void EncodeSignedVarint(long value, StringBuilder sb)
    {
        var unsigned = value < 0 ? ~(value << 1) : (value << 1);
        EncodeUnsignedVarint(unsigned, sb);
    }

    private static int DecodeChar(char c)
    {
        var idx = (int)c - 45;
        if (idx < 0 || idx >= DecodingTable.Length)
            throw new ArgumentException($"Invalid character '{c}' in flexible polyline encoding.");
        var val = DecodingTable[idx];
        if (val < 0)
            throw new ArgumentException($"Invalid character '{c}' in flexible polyline encoding.");
        return val;
    }
}
