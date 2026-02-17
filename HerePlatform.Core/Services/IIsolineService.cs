using HerePlatform.Core.Isoline;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Calculate reachable areas (isochrones and isodistances) via the HERE Isoline Routing API v8.
/// </summary>
public interface IIsolineService
{
    /// <summary>
    /// Compute one or more isolines from a center point within given range values.
    /// </summary>
    Task<IsolineResult> CalculateIsolineAsync(IsolineRequest request);
}
