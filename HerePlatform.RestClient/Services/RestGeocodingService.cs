using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geocoding;
using HerePlatform.Core.Services;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestGeocodingService : IGeocodingService
{
    private const string GeocodeBaseUrl = "https://geocode.search.hereapi.com/v1/geocode";
    private const string ReverseGeocodeBaseUrl = "https://revgeocode.search.hereapi.com/v1/revgeocode";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestGeocodingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GeocodeResult> GeocodeAsync(string query, GeocodeOptions? options = null)
    {
        var qs = HereApiHelper.BuildQueryString(
            ("q", query),
            ("lang", options?.Lang),
            ("limit", (options?.Limit ?? 5).ToString()),
            ("in", options?.BoundingBox is not null ? $"bbox:{options.BoundingBox}" : null));

        var url = $"{GeocodeBaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.GetAsync(url).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "geocoding");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereGeocodeResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse);
    }

    public async Task<GeocodeResult> ReverseGeocodeAsync(LatLngLiteral position, GeocodeOptions? options = null)
    {
        var qs = HereApiHelper.BuildQueryString(
            ("at", HereApiHelper.FormatCoord(position)),
            ("lang", options?.Lang),
            ("limit", (options?.Limit ?? 5).ToString()));

        var url = $"{ReverseGeocodeBaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.GetAsync(url).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "geocoding");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereGeocodeResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse);
    }

    private static GeocodeResult MapToResult(HereGeocodeResponse? hereResponse)
    {
        if (hereResponse?.Items is null or { Count: 0 })
            return new GeocodeResult { Items = [] };

        return new GeocodeResult
        {
            Items = hereResponse.Items.Select(item => new GeocodeItem
            {
                Title = item.Title,
                Address = item.Address?.Label,
                ResultType = item.ResultType,
                Position = item.Position is not null
                    ? new LatLngLiteral(item.Position.Lat, item.Position.Lng)
                    : null
            }).ToList()
        };
    }
}
