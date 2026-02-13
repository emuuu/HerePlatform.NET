using System;
using System.Collections.Generic;
using System.Globalization;

namespace HerePlatformComponents.Maps.Utilities;

/// <summary>
/// Parser for Well-Known Text (WKT) geometry strings.
/// </summary>
public static class WktParser
{
    /// <summary>
    /// Parse a WKT POINT string to a LatLngLiteral.
    /// </summary>
    public static LatLngLiteral? ParsePoint(string wkt)
    {
        if (string.IsNullOrWhiteSpace(wkt)) return null;

        var trimmed = wkt.Trim();
        if (!trimmed.StartsWith("POINT", StringComparison.OrdinalIgnoreCase)) return null;

        var coords = ExtractCoordinateBlock(trimmed);
        if (coords == null) return null;

        var parts = coords.Trim().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return null;

        if (double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lng) &&
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
        {
            return new LatLngLiteral(lat, lng);
        }

        return null;
    }

    /// <summary>
    /// Parse a WKT LINESTRING to a list of LatLngLiteral.
    /// </summary>
    public static List<LatLngLiteral> ParseLineString(string wkt)
    {
        if (string.IsNullOrWhiteSpace(wkt)) return new List<LatLngLiteral>();

        var trimmed = wkt.Trim();
        if (!trimmed.StartsWith("LINESTRING", StringComparison.OrdinalIgnoreCase))
            return new List<LatLngLiteral>();

        var coords = ExtractCoordinateBlock(trimmed);
        if (coords == null) return new List<LatLngLiteral>();

        return ParseCoordinateList(coords);
    }

    /// <summary>
    /// Parse a WKT POLYGON to a list of rings (outer ring + optional holes).
    /// </summary>
    public static List<List<LatLngLiteral>> ParsePolygon(string wkt)
    {
        if (string.IsNullOrWhiteSpace(wkt)) return new List<List<LatLngLiteral>>();

        var trimmed = wkt.Trim();
        if (!trimmed.StartsWith("POLYGON", StringComparison.OrdinalIgnoreCase))
            return new List<List<LatLngLiteral>>();

        // Extract everything between outer parentheses
        var start = trimmed.IndexOf("((", StringComparison.Ordinal);
        var end = trimmed.LastIndexOf("))", StringComparison.Ordinal);
        if (start < 0 || end < 0) return new List<List<LatLngLiteral>>();

        var inner = trimmed.Substring(start + 2, end - start - 2);
        var rings = inner.Split(new[] { "),(" }, StringSplitOptions.RemoveEmptyEntries);

        var result = new List<List<LatLngLiteral>>();
        foreach (var ring in rings)
        {
            var cleaned = ring.Trim('(', ')', ' ');
            result.Add(ParseCoordinateList(cleaned));
        }

        return result;
    }

    /// <summary>
    /// Parse a WKT MULTIPOINT to a list of LatLngLiteral.
    /// </summary>
    public static List<LatLngLiteral> ParseMultiPoint(string wkt)
    {
        if (string.IsNullOrWhiteSpace(wkt)) return new List<LatLngLiteral>();

        var trimmed = wkt.Trim();
        if (!trimmed.StartsWith("MULTIPOINT", StringComparison.OrdinalIgnoreCase))
            return new List<LatLngLiteral>();

        var coords = ExtractCoordinateBlock(trimmed);
        if (coords == null) return new List<LatLngLiteral>();

        // Handle both MULTIPOINT((x y),(x y)) and MULTIPOINT(x y, x y) formats
        var cleaned = coords.Replace("(", "").Replace(")", "");
        return ParseCoordinateList(cleaned);
    }

    /// <summary>
    /// Parse a WKT MULTILINESTRING to a list of line strings.
    /// </summary>
    public static List<List<LatLngLiteral>> ParseMultiLineString(string wkt)
    {
        if (string.IsNullOrWhiteSpace(wkt)) return new List<List<LatLngLiteral>>();

        var trimmed = wkt.Trim();
        if (!trimmed.StartsWith("MULTILINESTRING", StringComparison.OrdinalIgnoreCase))
            return new List<List<LatLngLiteral>>();

        var start = trimmed.IndexOf("((", StringComparison.Ordinal);
        var end = trimmed.LastIndexOf("))", StringComparison.Ordinal);
        if (start < 0 || end < 0) return new List<List<LatLngLiteral>>();

        var inner = trimmed.Substring(start + 2, end - start - 2);
        var lines = inner.Split(new[] { "),(" }, StringSplitOptions.RemoveEmptyEntries);

        var result = new List<List<LatLngLiteral>>();
        foreach (var line in lines)
        {
            var cleaned = line.Trim('(', ')', ' ');
            result.Add(ParseCoordinateList(cleaned));
        }

        return result;
    }

    private static string? ExtractCoordinateBlock(string wkt)
    {
        var openParen = wkt.IndexOf('(');
        var closeParen = wkt.LastIndexOf(')');
        if (openParen < 0 || closeParen < 0 || closeParen <= openParen) return null;

        return wkt.Substring(openParen + 1, closeParen - openParen - 1);
    }

    private static List<LatLngLiteral> ParseCoordinateList(string coords)
    {
        var result = new List<LatLngLiteral>();
        var pairs = coords.Split(',');

        foreach (var pair in pairs)
        {
            var parts = pair.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 &&
                double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lng) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
            {
                result.Add(new LatLngLiteral(lat, lng));
            }
        }

        return result;
    }
}
