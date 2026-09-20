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

    public async Task<GeocodeResult> GeocodeAsync(string query, GeocodeOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var qs = HereApiHelper.BuildQueryString(
            ("q", query),
            ("lang", options?.Lang),
            ("limit", (options?.Limit ?? 5).ToString()),
            ("in", options?.BoundingBox is not null ? $"bbox:{options.BoundingBox}" : null));

        var url = $"{GeocodeBaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient(HereApiHelper.ClientName);
        using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

        await HereApiHelper.EnsureSuccessOrThrowAsync(response, "geocoding", cancellationToken).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereGeocodeResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse);
    }

    public async Task<GeocodeResult> ReverseGeocodeAsync(LatLngLiteral position, GeocodeOptions? options = null, CancellationToken cancellationToken = default)
    {
        var qs = HereApiHelper.BuildQueryString(
            ("at", HereApiHelper.FormatCoord(position)),
            ("lang", options?.Lang),
            ("limit", (options?.Limit ?? 5).ToString()));

        var url = $"{ReverseGeocodeBaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient(HereApiHelper.ClientName);
        using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

        await HereApiHelper.EnsureSuccessOrThrowAsync(response, "geocoding", cancellationToken).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
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
                AddressDetails = MapAddress(item.Address),
                ResultType = item.ResultType,
                Position = item.Position is not null
                    ? new LatLngLiteral(item.Position.Lat, item.Position.Lng)
                    : null
            }).ToList()
        };
    }

    private static GeocodeAddress? MapAddress(HereAddress? address)
    {
        if (address is null)
            return null;

        return new GeocodeAddress
        {
            Label = address.Label,
            CountryCode = address.CountryCode,
            CountryName = address.CountryName,
            State = address.State,
            StateCode = address.StateCode,
            County = address.County,
            CountyCode = address.CountyCode,
            City = address.City,
            District = address.District,
            Street = address.Street,
            PostalCode = address.PostalCode,
            HouseNumber = address.HouseNumber
        };
    }
}
