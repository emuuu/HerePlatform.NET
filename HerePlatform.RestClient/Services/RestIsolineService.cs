using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Isoline;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Services;
using HerePlatform.Core.Utilities;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestIsolineService : IIsolineService
{
    private const string BaseUrl = "https://isoline.router.hereapi.com/v8/isolines";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestIsolineService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IsolineResult> CalculateIsolineAsync(IsolineRequest request)
    {
        var rangeValues = request.Ranges is { Count: > 0 }
            ? string.Join(",", request.Ranges)
            : null;

        var parameters = new List<(string key, string? value)>
        {
            ("origin", HereApiHelper.FormatCoord(request.Center)),
            ("range[values]", rangeValues),
            ("range[type]", HereApiHelper.GetEnumMemberValue(request.RangeType)),
            ("transportMode", HereApiHelper.GetEnumMemberValue(request.TransportMode)),
            ("routingMode", HereApiHelper.GetEnumMemberValue(request.RoutingMode)),
            ("departureTime", request.DepartureTime)
        };

        if (request.Avoid != RoutingAvoidFeature.None)
        {
            var avoidFeatures = HereApiHelper.GetAvoidFeatures(request.Avoid);
            if (avoidFeatures.Length > 0)
                parameters.Add(("avoid[features]", string.Join(",", avoidFeatures)));
        }

        var qs = HereApiHelper.BuildQueryString(parameters.ToArray());
        var url = $"{BaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.GetAsync(url).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "isoline");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereIsolineResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse);
    }

    private static IsolineResult MapToResult(HereIsolineResponse? hereResponse)
    {
        if (hereResponse?.Isolines is null or { Count: 0 })
            return new IsolineResult { Isolines = [] };

        return new IsolineResult
        {
            Isolines = hereResponse.Isolines.Select(iso =>
            {
                var polygon = new IsolinePolygon
                {
                    Range = iso.Range?.Value ?? 0
                };

                // Take the first polygon's outer ring
                var encoded = iso.Polygons?.FirstOrDefault()?.Outer;
                if (!string.IsNullOrEmpty(encoded))
                {
                    polygon.EncodedPolyline = encoded;
                    try
                    {
                        polygon.Polygon = FlexiblePolyline.Decode(encoded);
                    }
                    catch (Exception)
                    {
                        // Leave Polygon null if decoding fails
                    }
                }

                return polygon;
            }).ToList()
        };
    }
}
