using HerePlatform.Core.Traffic;

namespace HerePlatform.Core.Services;

/// <summary>
/// Real-time traffic incidents and flow data via the HERE Traffic API v7.
/// </summary>
[HereApi("Traffic API", "v7")]
public interface ITrafficService
{
    /// <summary>
    /// Get traffic incidents (accidents, construction, congestion, etc.) within a bounding box.
    /// </summary>
    /// <param name="north">Northern latitude of bounding box.</param>
    /// <param name="south">Southern latitude of bounding box.</param>
    /// <param name="east">Eastern longitude of bounding box.</param>
    /// <param name="west">Western longitude of bounding box.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TrafficIncidentsResult> GetTrafficIncidentsAsync(double north, double south, double east, double west, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get real-time traffic flow data (speed, jam factor) within a bounding box.
    /// </summary>
    /// <param name="north">Northern latitude of bounding box.</param>
    /// <param name="south">Southern latitude of bounding box.</param>
    /// <param name="east">Eastern longitude of bounding box.</param>
    /// <param name="west">Western longitude of bounding box.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TrafficFlowResult> GetTrafficFlowAsync(double north, double south, double east, double west, CancellationToken cancellationToken = default);
}
