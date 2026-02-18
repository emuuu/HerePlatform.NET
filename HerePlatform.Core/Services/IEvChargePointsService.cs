using HerePlatform.Core.EvChargePoints;

namespace HerePlatform.Core.Services;

/// <summary>
/// Search for EV charging stations via the HERE EV Charge Points API v3.
/// </summary>
[HereApi("EV Charge Points API", "v3")]
public interface IEvChargePointsService
{
    /// <summary>
    /// Search for EV charging stations near a position.
    /// </summary>
    Task<EvChargePointsResult> SearchStationsAsync(EvChargePointsRequest request, CancellationToken cancellationToken = default);
}
