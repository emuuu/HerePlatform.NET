using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Result of a route calculation.
/// </summary>
public class RoutingResult
{
    /// <summary>
    /// Calculated routes.
    /// </summary>
    public List<Route>? Routes { get; set; }
}

/// <summary>
/// A single calculated route.
/// </summary>
public class Route
{
    /// <summary>
    /// Route sections (legs between waypoints).
    /// </summary>
    public List<RouteSection>? Sections { get; set; }
}

/// <summary>
/// A section of a route between two waypoints.
/// </summary>
public class RouteSection
{
    /// <summary>
    /// Encoded flexible polyline for this section.
    /// </summary>
    public string? Polyline { get; set; }

    /// <summary>
    /// Decoded polyline coordinates (populated by service).
    /// </summary>
    public List<LatLngLiteral>? DecodedPolyline { get; set; }

    /// <summary>
    /// Section summary.
    /// </summary>
    public RouteSummary? Summary { get; set; }

    /// <summary>
    /// Transport mode used for this section.
    /// </summary>
    public string? Transport { get; set; }
}

/// <summary>
/// Summary information for a route section.
/// </summary>
public class RouteSummary
{
    /// <summary>
    /// Duration in seconds.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Length in meters.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Base duration without traffic in seconds.
    /// </summary>
    public int? BaseDuration { get; set; }
}
