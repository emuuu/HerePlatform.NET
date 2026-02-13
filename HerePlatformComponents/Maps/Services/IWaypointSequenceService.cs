using HerePlatformComponents.Maps.Services.WaypointSequence;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

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
