using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps;

public class PolygonOptions : ListableEntityOptionsBase
{
    /// <summary>
    /// The outer boundary path as a sequence of {lat, lng} points.
    /// Will be converted to H.geo.Polygon via H.geo.LineString in JS.
    /// </summary>
    public IEnumerable<LatLngLiteral>? Path { get; set; }

    /// <summary>
    /// Visual style for the polygon (stroke and fill).
    /// </summary>
    public StyleOptions? Style { get; set; }

    /// <summary>
    /// Extrusion height in meters (3D rendering).
    /// </summary>
    public double? Extrusion { get; set; }

    /// <summary>
    /// Elevation in meters (3D rendering).
    /// </summary>
    public double? Elevation { get; set; }
}
