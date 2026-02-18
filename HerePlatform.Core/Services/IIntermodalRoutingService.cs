using HerePlatform.Core.IntermodalRouting;

namespace HerePlatform.Core.Services;

/// <summary>
/// Calculate combined transport routes via the HERE Intermodal Routing API v8.
/// </summary>
[HereApi("Intermodal Routing API", "v8")]
public interface IIntermodalRoutingService
{
    /// <summary>
    /// Calculate an intermodal route combining walking, transit, and other modes.
    /// </summary>
    Task<IntermodalRoutingResult> CalculateRouteAsync(IntermodalRoutingRequest request, CancellationToken cancellationToken = default);
}
