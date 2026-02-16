using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using System;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.MatrixRouting;

/// <summary>
/// Request parameters for matrix routing calculation.
/// </summary>
public class MatrixRoutingRequest
{
    /// <summary>
    /// Origin positions.
    /// </summary>
    public List<LatLngLiteral> Origins { get; set; } = [];

    /// <summary>
    /// Destination positions.
    /// </summary>
    public List<LatLngLiteral> Destinations { get; set; } = [];

    /// <summary>
    /// Transport mode (default: Car).
    /// </summary>
    public TransportMode TransportMode { get; set; } = TransportMode.Car;

    /// <summary>
    /// Routing optimization mode (default: Fast).
    /// </summary>
    public RoutingMode RoutingMode { get; set; } = RoutingMode.Fast;

    /// <summary>
    /// Optional departure time for time-aware routing.
    /// </summary>
    public DateTime? DepartureTime { get; set; }
}
