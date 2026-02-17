using HerePlatform.Core.WaypointSequence;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Optimize waypoint order for minimum travel time or distance via the HERE Waypoint Sequence API v8.
/// </summary>
public interface IWaypointSequenceService
{
    /// <summary>
    /// Given a start, end, and intermediate waypoints, find the optimal visiting order.
    /// </summary>
    Task<WaypointSequenceResult> OptimizeSequenceAsync(WaypointSequenceRequest request);
}
