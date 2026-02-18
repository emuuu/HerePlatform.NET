using HerePlatform.Core.Isoline;

namespace HerePlatform.Core.Services;

/// <summary>
/// Calculate reachable areas (isochrones and isodistances) via the HERE Isoline Routing API v8.
/// </summary>
[HereApi("Isoline Routing API", "v8")]
public interface IIsolineService
{
    /// <summary>
    /// Compute one or more isolines from a center point within given range values.
    /// </summary>
    Task<IsolineResult> CalculateIsolineAsync(IsolineRequest request, CancellationToken cancellationToken = default);
}
