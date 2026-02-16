using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Isoline;

/// <summary>
/// Request parameters for isoline (isochrone/isodistance) calculation.
/// </summary>
public class IsolineRequest
{
    /// <summary>
    /// Center position for the isoline calculation.
    /// </summary>
    public LatLngLiteral Center { get; set; }

    /// <summary>
    /// Range values (seconds for time, meters for distance).
    /// </summary>
    public List<int>? Ranges { get; set; }

    /// <summary>
    /// Type of range calculation.
    /// </summary>
    public IsolineRangeType RangeType { get; set; } = IsolineRangeType.Time;

    /// <summary>
    /// Transport mode (default: Car).
    /// </summary>
    public TransportMode TransportMode { get; set; } = TransportMode.Car;

    /// <summary>
    /// Routing optimization mode (default: Fast).
    /// </summary>
    public RoutingMode RoutingMode { get; set; } = RoutingMode.Fast;

    /// <summary>
    /// Features to avoid.
    /// </summary>
    public RoutingAvoidFeature Avoid { get; set; } = RoutingAvoidFeature.None;

    /// <summary>
    /// Optional departure time (ISO 8601 format).
    /// </summary>
    public string? DepartureTime { get; set; }
}
