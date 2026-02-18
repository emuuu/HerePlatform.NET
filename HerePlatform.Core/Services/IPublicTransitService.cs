using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Transit;

namespace HerePlatform.Core.Services;

/// <summary>
/// Transit departures and station search via the HERE Public Transit API v8.
/// </summary>
[HereApi("Public Transit API", "v8")]
public interface IPublicTransitService
{
    /// <summary>
    /// Get live departures from the nearest transit station to a given position.
    /// </summary>
    /// <param name="position">Position to find nearest station departures.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TransitDeparturesResult> GetDeparturesAsync(LatLngLiteral position, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for transit stations within a given radius around a position.
    /// </summary>
    /// <param name="position">Center of search area.</param>
    /// <param name="radiusMeters">Search radius in meters (default: 500).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TransitStationsResult> SearchStationsAsync(LatLngLiteral position, double radiusMeters = 500, CancellationToken cancellationToken = default);
}
