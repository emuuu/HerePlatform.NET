using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Services.Geocoding;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Service for HERE Geocoding and Reverse Geocoding.
/// </summary>
public interface IGeocodingService
{
    /// <summary>
    /// Forward geocode: address/place name to coordinates.
    /// </summary>
    Task<GeocodeResult> GeocodeAsync(string query, GeocodeOptions? options = null);

    /// <summary>
    /// Reverse geocode: coordinates to address.
    /// </summary>
    Task<GeocodeResult> ReverseGeocodeAsync(LatLngLiteral position, GeocodeOptions? options = null);
}
