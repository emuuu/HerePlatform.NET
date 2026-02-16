using HerePlatform.Core.Coordinates;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.Traffic;

/// <summary>
/// A traffic flow data item.
/// </summary>
public class TrafficFlowItem
{
    /// <summary>
    /// Current speed in km/h.
    /// </summary>
    public double CurrentSpeed { get; set; }

    /// <summary>
    /// Free-flow speed in km/h (speed without traffic).
    /// </summary>
    public double FreeFlowSpeed { get; set; }

    /// <summary>
    /// Jam factor (0=free flow, 10=standstill).
    /// </summary>
    public double JamFactor { get; set; }

    /// <summary>
    /// Road name.
    /// </summary>
    public string? RoadName { get; set; }

    /// <summary>
    /// Location along the road.
    /// </summary>
    public LatLngLiteral? Position { get; set; }
}

/// <summary>
/// Result containing traffic flow data.
/// </summary>
public class TrafficFlowResult
{
    /// <summary>
    /// List of traffic flow items.
    /// </summary>
    public List<TrafficFlowItem>? Items { get; set; }
}
