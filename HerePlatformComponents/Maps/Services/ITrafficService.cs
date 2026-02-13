using HerePlatformComponents.Maps.Services.Traffic;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Service for HERE Traffic data.
/// </summary>
public interface ITrafficService
{
    /// <summary>
    /// Get traffic incidents within a bounding box.
    /// </summary>
    Task<TrafficIncidentsResult> GetTrafficIncidentsAsync(double north, double south, double east, double west);

    /// <summary>
    /// Get traffic flow data within a bounding box.
    /// </summary>
    Task<TrafficFlowResult> GetTrafficFlowAsync(double north, double south, double east, double west);
}
