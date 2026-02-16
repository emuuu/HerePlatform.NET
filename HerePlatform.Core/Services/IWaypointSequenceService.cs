using HerePlatform.Core.WaypointSequence;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Service for optimizing waypoint sequences using HERE API.
/// </summary>
public interface IWaypointSequenceService
{
    /// <summary>
    /// Optimize the sequence of waypoints for minimum travel time/distance.
    /// </summary>
    Task<WaypointSequenceResult> OptimizeSequenceAsync(WaypointSequenceRequest request);
}
