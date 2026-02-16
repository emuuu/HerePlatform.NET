using HerePlatform.Core.Isoline;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Service for HERE Isoline Routing API v8 (isochrones/isodistances).
/// </summary>
public interface IIsolineService
{
    /// <summary>
    /// Calculate isoline (reachable area from a center point within given range).
    /// </summary>
    Task<IsolineResult> CalculateIsolineAsync(IsolineRequest request);
}
