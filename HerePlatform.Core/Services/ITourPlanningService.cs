using HerePlatform.Core.TourPlanning;

namespace HerePlatform.Core.Services;

/// <summary>
/// Optimize fleet vehicle tours via the HERE Tour Planning API v3.
/// </summary>
[HereApi("Tour Planning API", "v3")]
public interface ITourPlanningService
{
    /// <summary>
    /// Solve a tour planning problem synchronously.
    /// </summary>
    Task<TourPlanningResult> SolveAsync(TourPlanningProblem problem, CancellationToken cancellationToken = default);
}
