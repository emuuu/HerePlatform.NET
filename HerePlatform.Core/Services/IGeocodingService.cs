using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geocoding;

namespace HerePlatform.Core.Services;

/// <summary>
/// Forward and reverse geocoding via the HERE Geocoding &amp; Search API v7.
/// </summary>
[HereApi("Geocoding & Search API", "v7")]
public interface IGeocodingService
{
    /// <summary>
    /// Convert a free-text address or place name into geographic coordinates.
    /// </summary>
    /// <param name="query">Free-text address or place name.</param>
    /// <param name="options">Optional geocoding parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<GeocodeResult> GeocodeAsync(string query, GeocodeOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert geographic coordinates into an address.
    /// </summary>
    /// <param name="position">Geographic coordinates (lat, lng).</param>
    /// <param name="options">Optional geocoding parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<GeocodeResult> ReverseGeocodeAsync(LatLngLiteral position, GeocodeOptions? options = null, CancellationToken cancellationToken = default);
}
