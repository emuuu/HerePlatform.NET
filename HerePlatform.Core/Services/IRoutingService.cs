using HerePlatform.Core.Routing;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

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
