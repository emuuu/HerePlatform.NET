using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Request parameters for route calculation.
/// </summary>
public class RoutingRequest
{
    /// <summary>
    /// Origin position.
    /// </summary>
    public LatLngLiteral Origin { get; set; }

    /// <summary>
    /// Destination position.
    /// </summary>
    public LatLngLiteral Destination { get; set; }

    /// <summary>
    /// Optional via waypoints.
    /// </summary>
    public List<LatLngLiteral>? Via { get; set; }

    /// <summary>
    /// Transport mode (default: Car).
    /// </summary>
    public TransportMode TransportMode { get; set; } = TransportMode.Car;

    /// <summary>
    /// Routing optimization mode (default: Fast).
    /// </summary>
    public RoutingMode RoutingMode { get; set; } = RoutingMode.Fast;

    /// <summary>
    /// Whether to return the route polyline.
    /// </summary>
    public bool ReturnPolyline { get; set; } = true;

    /// <summary>
    /// Number of alternative routes to calculate.
    /// </summary>
    public int Alternatives { get; set; }

    /// <summary>
    /// Features to avoid.
    /// </summary>
    public RoutingAvoidFeature Avoid { get; set; } = RoutingAvoidFeature.None;
}
