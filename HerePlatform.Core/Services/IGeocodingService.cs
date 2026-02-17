using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geocoding;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Forward and reverse geocoding via the HERE Geocoding &amp; Search API v1.
/// </summary>
public interface IGeocodingService
{
    /// <summary>
    /// Convert a free-text address or place name into geographic coordinates.
    /// </summary>
    /// <param name="query">Free-text address or place name.</param>
    /// <param name="options">Optional geocoding parameters.</param>
    Task<GeocodeResult> GeocodeAsync(string query, GeocodeOptions? options = null);

    /// <summary>
    /// Convert geographic coordinates into an address.
    /// </summary>
    /// <param name="position">Geographic coordinates (lat, lng).</param>
    /// <param name="options">Optional geocoding parameters.</param>
    Task<GeocodeResult> ReverseGeocodeAsync(LatLngLiteral position, GeocodeOptions? options = null);
}
