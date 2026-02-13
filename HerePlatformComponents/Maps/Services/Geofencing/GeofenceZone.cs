using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Geofencing;

/// <summary>
/// A geofence zone defined by a polygon or circle.
/// </summary>
public class GeofenceZone
{
    /// <summary>
    /// Unique identifier for the zone.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Zone type: "polygon" or "circle".
    /// </summary>
    public string Type { get; set; } = "polygon";

    /// <summary>
    /// Polygon vertices (for polygon type).
    /// </summary>
    public List<LatLngLiteral>? Vertices { get; set; }

    /// <summary>
    /// Center position (for circle type).
    /// </summary>
    public LatLngLiteral? Center { get; set; }

    /// <summary>
    /// Radius in meters (for circle type).
    /// </summary>
    public double Radius { get; set; }
}

/// <summary>
/// Result of a geofence check.
/// </summary>
public class GeofenceCheckResult
{
    /// <summary>
    /// Whether the position is inside any of the checked zones.
    /// </summary>
    public bool IsInside { get; set; }

    /// <summary>
    /// IDs of zones the position is inside.
    /// </summary>
    public List<string>? MatchedZoneIds { get; set; }
}
