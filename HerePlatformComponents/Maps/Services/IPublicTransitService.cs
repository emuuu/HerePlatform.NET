using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Services.Transit;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Service for HERE Public Transit data.
/// </summary>
public interface IPublicTransitService
{
    /// <summary>
    /// Get departures from the nearest transit station.
    /// </summary>
    Task<TransitDeparturesResult> GetDeparturesAsync(LatLngLiteral position);

    /// <summary>
    /// Search for transit stations near a position.
    /// </summary>
    Task<TransitStationsResult> SearchStationsAsync(LatLngLiteral position, double radiusMeters = 500);
}
