using HerePlatform.Core.Traffic;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Real-time traffic incidents and flow data via the HERE Traffic API v7.
/// </summary>
public interface ITrafficService
{
    /// <summary>
    /// Get traffic incidents (accidents, construction, congestion, etc.) within a bounding box.
    /// </summary>
    /// <param name="north">Northern latitude of bounding box.</param>
    /// <param name="south">Southern latitude of bounding box.</param>
    /// <param name="east">Eastern longitude of bounding box.</param>
    /// <param name="west">Western longitude of bounding box.</param>
    Task<TrafficIncidentsResult> GetTrafficIncidentsAsync(double north, double south, double east, double west);

    /// <summary>
    /// Get real-time traffic flow data (speed, jam factor) within a bounding box.
    /// </summary>
    /// <param name="north">Northern latitude of bounding box.</param>
    /// <param name="south">Southern latitude of bounding box.</param>
    /// <param name="east">Eastern longitude of bounding box.</param>
    /// <param name="west">Western longitude of bounding box.</param>
    Task<TrafficFlowResult> GetTrafficFlowAsync(double north, double south, double east, double west);
}
