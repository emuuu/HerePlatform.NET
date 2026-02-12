using HerePlatformComponents.Serialization;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Represents a geographic coordinate with latitude and longitude.
/// Compatible with HERE Maps {lat, lng} format.
/// </summary>
[DebuggerDisplay("{Lat}, {Lng}")]
[StructLayout(LayoutKind.Explicit, Size = sizeof(double) * 2)]
[JsonConverter(typeof(LatLngLiteralConverter))]
public readonly struct LatLngLiteral : IEquatable<LatLngLiteral>
{
    [FieldOffset(0)]
    public readonly double Lat;

    [FieldOffset(sizeof(double))]
    public readonly double Lng;

    public LatLngLiteral(double lat, double lng)
    {
        if (lat is < -90 or > 90)
            throw new ArgumentException("Latitude values can only range from -90 to 90!", nameof(lat));

        if (lng is < -180 or > 180)
            throw new ArgumentException("Longitude values can only range from -180 to 180!", nameof(lng));

        Lat = lat;
        Lng = lng;
    }

    public bool Equals(LatLngLiteral other)
    {
        return Lat.Equals(other.Lat) && Lng.Equals(other.Lng);
    }

    public override bool Equals(object? obj)
    {
        return obj is LatLngLiteral other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Lat, Lng);
    }

    public static bool operator ==(LatLngLiteral left, LatLngLiteral right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LatLngLiteral left, LatLngLiteral right)
    {
        return !left.Equals(right);
    }
}
