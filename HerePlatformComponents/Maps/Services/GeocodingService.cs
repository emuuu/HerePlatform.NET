using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geocoding;
using HerePlatform.Core.Services;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of IGeocodingService using HERE JS API's H.service.SearchService.
/// </summary>
public class GeocodingService : IGeocodingService
{
    private readonly IJSRuntime _js;

    public GeocodingService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<GeocodeResult> GeocodeAsync(string query, GeocodeOptions? options = null, CancellationToken cancellationToken = default)
    {
        GeocodeResult? result;
        try
        {
            result = await _js.InvokeAsync<GeocodeResult>(
                JsInteropIdentifiers.Geocode,
                query, options ?? new GeocodeOptions());
        }
        catch (JSException ex)
        {
            JsAuthErrorHelper.ThrowIfAuthError(ex, "geocoding");
            throw;
        }

        return result ?? new GeocodeResult();
    }

    public async Task<GeocodeResult> ReverseGeocodeAsync(LatLngLiteral position, GeocodeOptions? options = null, CancellationToken cancellationToken = default)
    {
        GeocodeResult? result;
        try
        {
            result = await _js.InvokeAsync<GeocodeResult>(
                JsInteropIdentifiers.ReverseGeocode,
                position, options ?? new GeocodeOptions());
        }
        catch (JSException ex)
        {
            JsAuthErrorHelper.ThrowIfAuthError(ex, "reverse-geocoding");
            throw;
        }

        return result ?? new GeocodeResult();
    }
}
