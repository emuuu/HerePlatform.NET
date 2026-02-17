using System.Globalization;
using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Services;
using HerePlatform.Core.Traffic;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestTrafficService : ITrafficService
{
    private const string IncidentsBaseUrl = "https://data.traffic.hereapi.com/v7/incidents";
    private const string FlowBaseUrl = "https://data.traffic.hereapi.com/v7/flow";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestTrafficService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TrafficIncidentsResult> GetTrafficIncidentsAsync(
        double north, double south, double east, double west)
    {
        var qs = HereApiHelper.BuildQueryString(
            ("in", string.Create(CultureInfo.InvariantCulture, $"bbox:{west},{south},{east},{north}")),
            ("locationReferencing", "shape"));

        var url = $"{IncidentsBaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.GetAsync(url).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "traffic");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereTrafficIncidentsResponse>(json, HereJsonDefaults.Options);

        return MapIncidents(hereResponse);
    }

    public async Task<TrafficFlowResult> GetTrafficFlowAsync(
        double north, double south, double east, double west)
    {
        var qs = HereApiHelper.BuildQueryString(
            ("in", string.Create(CultureInfo.InvariantCulture, $"bbox:{west},{south},{east},{north}")),
            ("locationReferencing", "shape"));

        var url = $"{FlowBaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.GetAsync(url).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "traffic");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereTrafficFlowResponse>(json, HereJsonDefaults.Options);

        return MapFlow(hereResponse);
    }

    private static TrafficIncidentsResult MapIncidents(HereTrafficIncidentsResponse? hereResponse)
    {
        if (hereResponse?.Results is null or { Count: 0 })
            return new TrafficIncidentsResult { Incidents = [] };

        return new TrafficIncidentsResult
        {
            Incidents = hereResponse.Results.Select(r =>
            {
                var details = r.IncidentDetails;
                return new TrafficIncident
                {
                    Type = details?.Type,
                    Severity = MapCriticality(details?.Criticality),
                    Description = details?.Description,
                    StartTime = details?.StartTime,
                    EndTime = details?.EndTime,
                    RoadName = r.Location?.Description,
                    Position = r.Location?.Shape is not null
                        ? new LatLngLiteral(r.Location.Shape.Lat, r.Location.Shape.Lng)
                        : null
                };
            }).ToList()
        };
    }

    private static TrafficFlowResult MapFlow(HereTrafficFlowResponse? hereResponse)
    {
        if (hereResponse?.Results is null or { Count: 0 })
            return new TrafficFlowResult { Items = [] };

        return new TrafficFlowResult
        {
            Items = hereResponse.Results.Select(r => new TrafficFlowItem
            {
                CurrentSpeed = r.CurrentFlow?.Speed ?? 0,
                FreeFlowSpeed = r.CurrentFlow?.FreeFlow ?? 0,
                JamFactor = r.CurrentFlow?.JamFactor ?? 0,
                RoadName = r.Location?.Description,
                Position = r.Location?.Shape is not null
                    ? new LatLngLiteral(r.Location.Shape.Lat, r.Location.Shape.Lng)
                    : null
            }).ToList()
        };
    }

    private static int MapCriticality(string? criticality) => criticality switch
    {
        "critical" => 4,
        "major" => 3,
        "minor" => 2,
        "lowImpact" => 1,
        _ => 0
    };
}
