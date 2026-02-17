using HerePlatform.Core.Routing;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Route calculation via the HERE Routing API v8.
/// Supports car, truck, pedestrian, bicycle, scooter, and EV routing.
/// </summary>
public interface IRoutingService
{
    /// <summary>
    /// Calculate a route between an origin and destination, with optional via waypoints, avoidance features, truck, and EV parameters.
    /// </summary>
    Task<RoutingResult> CalculateRouteAsync(RoutingRequest request);
}
