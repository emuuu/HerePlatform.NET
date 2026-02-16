using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Places;
using HerePlatform.Core.Services;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestPlacesService : IPlacesService
{
    private const string DiscoverBaseUrl = "https://discover.search.hereapi.com/v1/discover";
    private const string BrowseBaseUrl = "https://browse.search.hereapi.com/v1/browse";
    private const string LookupBaseUrl = "https://lookup.search.hereapi.com/v1/lookup";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestPlacesService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PlacesResult> DiscoverAsync(PlacesRequest request)
    {
        var qs = HereApiHelper.BuildQueryString(
            ("q", request.Query),
            ("at", request.At.HasValue ? $"{request.At.Value.Lat},{request.At.Value.Lng}" : null),
            ("in", request.BoundingBox is not null ? $"bbox:{request.BoundingBox}" : null),
            ("limit", request.Limit.ToString()),
            ("lang", request.Lang));

        var url = $"{DiscoverBaseUrl}?{qs}";
        return await ExecuteRequest(url);
    }

    public async Task<PlacesResult> BrowseAsync(PlacesRequest request)
    {
        var categories = request.Categories is { Count: > 0 }
            ? string.Join(",", request.Categories)
            : null;

        var qs = HereApiHelper.BuildQueryString(
            ("at", request.At.HasValue ? $"{request.At.Value.Lat},{request.At.Value.Lng}" : null),
            ("in", request.BoundingBox is not null ? $"bbox:{request.BoundingBox}" : null),
            ("categories", categories),
            ("limit", request.Limit.ToString()),
            ("lang", request.Lang));

        var url = $"{BrowseBaseUrl}?{qs}";
        return await ExecuteRequest(url);
    }

    public async Task<PlacesResult> LookupAsync(PlacesRequest request)
    {
        var qs = HereApiHelper.BuildQueryString(
            ("id", request.Id),
            ("lang", request.Lang));

        var url = $"{LookupBaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        var response = await client.GetAsync(url);

        HereApiHelper.EnsureAuthSuccess(response, "places");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        // Lookup returns a single item, not a list
        var hereItem = JsonSerializer.Deserialize<HerePlaceItem>(json, HereJsonDefaults.Options);
        if (hereItem is null)
            return new PlacesResult { Items = [] };

        return new PlacesResult { Items = [MapToPlaceItem(hereItem)] };
    }

    private async Task<PlacesResult> ExecuteRequest(string url)
    {
        var client = _httpClientFactory.CreateClient("HereApi");
        var response = await client.GetAsync(url);

        HereApiHelper.EnsureAuthSuccess(response, "places");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var hereResponse = JsonSerializer.Deserialize<HerePlacesResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse);
    }

    private static PlacesResult MapToResult(HerePlacesResponse? hereResponse)
    {
        if (hereResponse?.Items is null or { Count: 0 })
            return new PlacesResult { Items = [] };

        return new PlacesResult
        {
            Items = hereResponse.Items.Select(MapToPlaceItem).ToList()
        };
    }

    private static PlaceItem MapToPlaceItem(HerePlaceItem item)
    {
        var contacts = new List<PlaceContact>();
        if (item.Contacts is { Count: > 0 })
        {
            foreach (var contactList in item.Contacts)
            {
                if (contactList.Phone is not null)
                    foreach (var p in contactList.Phone)
                        if (p.Value is not null)
                            contacts.Add(new PlaceContact { Type = "phone", Value = p.Value });

                if (contactList.Www is not null)
                    foreach (var w in contactList.Www)
                        if (w.Value is not null)
                            contacts.Add(new PlaceContact { Type = "website", Value = w.Value });

                if (contactList.Email is not null)
                    foreach (var e in contactList.Email)
                        if (e.Value is not null)
                            contacts.Add(new PlaceContact { Type = "email", Value = e.Value });
            }
        }

        return new PlaceItem
        {
            PlaceId = item.Id,
            Title = item.Title,
            Address = item.Address?.Label,
            Position = item.Position is not null
                ? new LatLngLiteral(item.Position.Lat, item.Position.Lng)
                : null,
            Categories = item.Categories?.Select(c => c.Name!)
                .Where(n => n is not null).ToList(),
            OpeningHours = item.OpeningHours?.FirstOrDefault()?.Text?.FirstOrDefault(),
            Contacts = contacts.Count > 0 ? contacts : null,
            Distance = item.Distance
        };
    }
}
