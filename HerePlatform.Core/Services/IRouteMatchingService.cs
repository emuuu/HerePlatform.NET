using HerePlatform.Core.RouteMatching;

namespace HerePlatform.Core.Services;

/// <summary>
/// Match GPS traces to road segments via the HERE Route Matching API v8.
/// </summary>
[HereApi("Route Matching API", "v8")]
public interface IRouteMatchingService
{
    /// <summary>
    /// Match a sequence of GPS trace points to road segments.
    /// </summary>
    /// <param name="request">GPS trace and transport mode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<RouteMatchingResult> MatchRouteAsync(RouteMatchingRequest request, CancellationToken cancellationToken = default);
}
