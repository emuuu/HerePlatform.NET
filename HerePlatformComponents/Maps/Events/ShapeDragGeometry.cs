using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Events;

/// <summary>
/// Geometry data sent from JS after a shape drag ends.
/// Only the fields relevant to the shape type are populated.
/// </summary>
public class ShapeDragGeometry
{
    /// <summary>Circle center latitude after drag.</summary>
    public double? CenterLat { get; set; }

    /// <summary>Circle center longitude after drag.</summary>
    public double? CenterLng { get; set; }

    /// <summary>Rect top after drag.</summary>
    public double? Top { get; set; }

    /// <summary>Rect left after drag.</summary>
    public double? Left { get; set; }

    /// <summary>Rect bottom after drag.</summary>
    public double? Bottom { get; set; }

    /// <summary>Rect right after drag.</summary>
    public double? Right { get; set; }

    /// <summary>Polygon/Polyline path after drag.</summary>
    public List<LatLngLiteral>? Path { get; set; }
}
