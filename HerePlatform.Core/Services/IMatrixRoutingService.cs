using HerePlatform.Core.MatrixRouting;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Compute travel time and distance matrices via the HERE Matrix Routing API v8.
/// </summary>
public interface IMatrixRoutingService
{
    /// <summary>
    /// Calculate a routing matrix between multiple origins and destinations. Returns duration and distance for each origin-destination pair.
    /// </summary>
    Task<MatrixRoutingResult> CalculateMatrixAsync(MatrixRoutingRequest request);
}
