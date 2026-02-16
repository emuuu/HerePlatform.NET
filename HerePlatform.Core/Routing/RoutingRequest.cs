using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatform.Core.Routing;

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

    /// <summary>
    /// When true, the response includes turn-by-turn instructions.
    /// </summary>
    public bool ReturnInstructions { get; set; }

    /// <summary>
    /// Truck-specific parameters (only used when TransportMode is Truck).
    /// </summary>
    public TruckOptions? Truck { get; set; }

    /// <summary>
    /// Electric vehicle parameters.
    /// </summary>
    public EvOptions? Ev { get; set; }
}
