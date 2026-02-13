using HerePlatformComponents.Maps.Services.Routing;
using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Services.WaypointSequence;

/// <summary>
/// Request for optimizing waypoint sequence.
/// </summary>
public class WaypointSequenceRequest
{
    /// <summary>
    /// Start position.
    /// </summary>
    public LatLngLiteral Start { get; set; }

    /// <summary>
    /// End position (can be same as Start for round-trip).
    /// </summary>
    public LatLngLiteral End { get; set; }

    /// <summary>
    /// Waypoints to optimize the sequence for.
    /// </summary>
    public List<LatLngLiteral>? Waypoints { get; set; }

    /// <summary>
    /// Transport mode for optimization.
    /// </summary>
    public TransportMode TransportMode { get; set; } = TransportMode.Car;
}

/// <summary>
/// Result of waypoint sequence optimization.
/// </summary>
public class WaypointSequenceResult
{
    /// <summary>
    /// Optimized waypoint indices (original ordering mapped to optimal).
    /// </summary>
    public List<int>? OptimizedIndices { get; set; }

    /// <summary>
    /// Optimized waypoints in the recommended order.
    /// </summary>
    public List<LatLngLiteral>? OptimizedWaypoints { get; set; }

    /// <summary>
    /// Total distance in meters.
    /// </summary>
    public int TotalDistance { get; set; }

    /// <summary>
    /// Total duration in seconds.
    /// </summary>
    public int TotalDuration { get; set; }
}
