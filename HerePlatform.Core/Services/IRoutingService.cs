using HerePlatform.Core.Routing;

namespace HerePlatform.Core.Services;

/// <summary>
/// Route calculation via the HERE Routing API v8.
/// Supports car, truck, pedestrian, bicycle, scooter, and EV routing.
/// </summary>
[HereApi("Routing API", "v8")]
public interface IRoutingService
{
    /// <summary>
    /// Calculate a route between an origin and destination, with optional via waypoints, avoidance features, truck, and EV parameters.
    /// </summary>
    Task<RoutingResult> CalculateRouteAsync(RoutingRequest request, CancellationToken cancellationToken = default);
}
