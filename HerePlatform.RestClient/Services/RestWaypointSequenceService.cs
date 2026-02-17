using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Services;
using HerePlatform.Core.WaypointSequence;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestWaypointSequenceService : IWaypointSequenceService
{
    private const string BaseUrl = "https://wps.hereapi.com/v8/findsequence2";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestWaypointSequenceService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<WaypointSequenceResult> OptimizeSequenceAsync(WaypointSequenceRequest request)
    {
        var parameters = new List<(string key, string? value)>
        {
            ("start", HereApiHelper.FormatCoord(request.Start)),
            ("end", HereApiHelper.FormatCoord(request.End)),
            ("mode", MapTransportMode(request.TransportMode))
        };

        if (request.Waypoints is { Count: > 0 })
        {
            for (int i = 0; i < request.Waypoints.Count; i++)
                parameters.Add(($"destination{i + 1}", HereApiHelper.FormatCoord(request.Waypoints[i])));
        }

        var qs = HereApiHelper.BuildQueryString(parameters.ToArray());
        var url = $"{BaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.GetAsync(url).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "waypoint-sequence");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereWaypointSequenceResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse);
    }

    private static string MapTransportMode(TransportMode mode) => mode switch
    {
        TransportMode.Truck => "fastest;truck",
        TransportMode.Pedestrian => "fastest;pedestrian",
        TransportMode.Bicycle => "fastest;bicycle",
        _ => "fastest;car"
    };

    private static WaypointSequenceResult MapToResult(HereWaypointSequenceResponse? response)
    {
        var result = response?.Results is { Count: > 0 } ? response.Results[0] : null;
        if (result?.Waypoints is null)
            return new WaypointSequenceResult();

        var destinations = result.Waypoints
            .Where(w => w.Id is not null && w.Id.StartsWith("destination", StringComparison.Ordinal))
            .OrderBy(w => w.Sequence)
            .ToList();

        var optimizedIndices = new List<int>();
        var optimizedWaypoints = new List<LatLngLiteral>();

        foreach (var wp in destinations)
        {
            if (int.TryParse(wp.Id!.AsSpan("destination".Length), out var num))
                optimizedIndices.Add(num - 1); // 1-based destination id → 0-based index
            optimizedWaypoints.Add(new LatLngLiteral(wp.Lat, wp.Lng));
        }

        return new WaypointSequenceResult
        {
            OptimizedIndices = optimizedIndices,
            OptimizedWaypoints = optimizedWaypoints,
            TotalDistance = result.Distance,
            TotalDuration = result.Time
        };
    }
}
