using HerePlatform.Core.Coordinates;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HerePlatformComponents.Maps.Utilities;

/// <summary>
/// Export geographic data as GeoJSON strings.
/// </summary>
public static class GeoJsonExporter
{
    /// <summary>
    /// Convert a point to a GeoJSON Feature string.
    /// </summary>
    public static string ToGeoJsonFeature(LatLngLiteral point, Dictionary<string, object>? properties = null)
    {
        var sb = new StringBuilder();
        sb.Append("{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":");
        AppendCoordinate(sb, point);
        sb.Append("},\"properties\":");
        AppendProperties(sb, properties);
        sb.Append('}');
        return sb.ToString();
    }

    /// <summary>
    /// Convert a line string to a GeoJSON Feature string.
    /// </summary>
    public static string ToLineStringFeature(IEnumerable<LatLngLiteral> lineString, Dictionary<string, object>? properties = null)
    {
        var sb = new StringBuilder();
        sb.Append("{\"type\":\"Feature\",\"geometry\":{\"type\":\"LineString\",\"coordinates\":");
        AppendCoordinateArray(sb, lineString);
        sb.Append("},\"properties\":");
        AppendProperties(sb, properties);
        sb.Append('}');
        return sb.ToString();
    }

    /// <summary>
    /// Convert a polygon (with optional holes) to a GeoJSON Feature string.
    /// </summary>
    public static string ToPolygonFeature(List<LatLngLiteral> exteriorRing, List<List<LatLngLiteral>>? holes = null, Dictionary<string, object>? properties = null)
    {
        var sb = new StringBuilder();
        sb.Append("{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[");
        AppendCoordinateArray(sb, exteriorRing);
        if (holes != null)
        {
            foreach (var hole in holes)
            {
                sb.Append(',');
                AppendCoordinateArray(sb, hole);
            }
        }
        sb.Append("]},\"properties\":");
        AppendProperties(sb, properties);
        sb.Append('}');
        return sb.ToString();
    }

    /// <summary>
    /// Create a GeoJSON FeatureCollection from multiple feature JSON strings.
    /// </summary>
    public static string ToFeatureCollection(IEnumerable<string> featureJsons)
    {
        var sb = new StringBuilder();
        sb.Append("{\"type\":\"FeatureCollection\",\"features\":[");
        bool first = true;
        foreach (var feature in featureJsons)
        {
            if (!first) sb.Append(',');
            sb.Append(feature);
            first = false;
        }
        sb.Append("]}");
        return sb.ToString();
    }

    private static void AppendCoordinate(StringBuilder sb, LatLngLiteral coord)
    {
        sb.Append('[');
        sb.Append(coord.Lng.ToString("G", CultureInfo.InvariantCulture));
        sb.Append(',');
        sb.Append(coord.Lat.ToString("G", CultureInfo.InvariantCulture));
        sb.Append(']');
    }

    private static void AppendCoordinateArray(StringBuilder sb, IEnumerable<LatLngLiteral> coords)
    {
        sb.Append('[');
        bool first = true;
        foreach (var coord in coords)
        {
            if (!first) sb.Append(',');
            AppendCoordinate(sb, coord);
            first = false;
        }
        sb.Append(']');
    }

    private static void AppendProperties(StringBuilder sb, Dictionary<string, object>? properties)
    {
        if (properties == null || properties.Count == 0)
        {
            sb.Append("{}");
            return;
        }

        sb.Append('{');
        bool first = true;
        foreach (var kvp in properties)
        {
            if (!first) sb.Append(',');
            sb.Append('"');
            sb.Append(EscapeJsonString(kvp.Key));
            sb.Append("\":");

            if (kvp.Value is string s)
            {
                sb.Append('"');
                sb.Append(EscapeJsonString(s));
                sb.Append('"');
            }
            else if (kvp.Value is bool b)
            {
                sb.Append(b ? "true" : "false");
            }
            else if (kvp.Value is int i)
            {
                sb.Append(i.ToString(CultureInfo.InvariantCulture));
            }
            else if (kvp.Value is double d)
            {
                sb.Append(d.ToString("G", CultureInfo.InvariantCulture));
            }
            else if (kvp.Value == null)
            {
                sb.Append("null");
            }
            else
            {
                sb.Append('"');
                sb.Append(EscapeJsonString(kvp.Value.ToString() ?? ""));
                sb.Append('"');
            }

            first = false;
        }
        sb.Append('}');
    }

    private static string EscapeJsonString(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }
}
