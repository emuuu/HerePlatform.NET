using HerePlatformComponents.Maps.Services.Routing;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Service for HERE Routing API v8.
/// </summary>
public interface IRoutingService
{
    /// <summary>
    /// Calculate a route between origin and destination.
    /// </summary>
    Task<RoutingResult> CalculateRouteAsync(RoutingRequest request);
}
