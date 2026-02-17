using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Services;
using HerePlatform.Core.Utilities;
using HerePlatform.RestClient.Internal;

namespace HerePlatform.RestClient.Services;

internal sealed class RestRoutingService : IRoutingService
{
    private const string BaseUrl = "https://router.hereapi.com/v8/routes";

    private readonly IHttpClientFactory _httpClientFactory;

    public RestRoutingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<RoutingResult> CalculateRouteAsync(RoutingRequest request)
    {
        var returnParts = new List<string> { "summary" };
        if (request.ReturnPolyline)
            returnParts.Add("polyline");
        if (request.ReturnInstructions)
            returnParts.Add("turnByTurnActions");

        var parameters = new List<(string key, string? value)>
        {
            ("origin", HereApiHelper.FormatCoord(request.Origin)),
            ("destination", HereApiHelper.FormatCoord(request.Destination)),
            ("transportMode", HereApiHelper.GetEnumMemberValue(request.TransportMode)),
            ("routingMode", HereApiHelper.GetEnumMemberValue(request.RoutingMode)),
            ("return", string.Join(",", returnParts)),
            ("alternatives", request.Alternatives > 0 ? request.Alternatives.ToString() : null)
        };

        // Via waypoints
        if (request.Via is { Count: > 0 })
        {
            foreach (var via in request.Via)
                parameters.Add(("via", HereApiHelper.FormatCoord(via)));
        }

        // Avoid features
        if (request.Avoid != RoutingAvoidFeature.None)
        {
            var avoidFeatures = HereApiHelper.GetAvoidFeatures(request.Avoid);
            if (avoidFeatures.Length > 0)
                parameters.Add(("avoid[features]", string.Join(",", avoidFeatures)));
        }

        // Truck options
        if (request.TransportMode == TransportMode.Truck && request.Truck is not null)
            AddTruckParameters(parameters, request.Truck);

        // EV options
        if (request.Ev is not null)
            AddEvParameters(parameters, request.Ev);

        var qs = HereApiHelper.BuildQueryString(parameters.ToArray());
        var url = $"{BaseUrl}?{qs}";

        var client = _httpClientFactory.CreateClient("HereApi");
        using var response = await client.GetAsync(url).ConfigureAwait(false);

        HereApiHelper.EnsureAuthSuccess(response, "routing");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var hereResponse = JsonSerializer.Deserialize<HereRoutingResponse>(json, HereJsonDefaults.Options);

        return MapToResult(hereResponse);
    }

    private static RoutingResult MapToResult(HereRoutingResponse? hereResponse)
    {
        if (hereResponse?.Routes is null or { Count: 0 })
            return new RoutingResult { Routes = [] };

        return new RoutingResult
        {
            Routes = hereResponse.Routes.Select(route => new Route
            {
                Sections = route.Sections?.Select(section =>
                {
                    var rs = new RouteSection
                    {
                        Polyline = section.Polyline,
                        Transport = section.Transport?.Mode,
                        Summary = section.Summary is not null ? new RouteSummary
                        {
                            Duration = section.Summary.Duration,
                            Length = section.Summary.Length,
                            BaseDuration = section.Summary.BaseDuration
                        } : null,
                        TurnByTurnActions = MapActions(section)
                    };

                    // Decode polyline
                    if (!string.IsNullOrEmpty(section.Polyline))
                    {
                        try
                        {
                            rs.DecodedPolyline = FlexiblePolyline.Decode(section.Polyline);
                        }
                        catch (Exception)
                        {
                            // If decoding fails, leave DecodedPolyline null
                        }
                    }

                    return rs;
                }).ToList()
            }).ToList()
        };
    }

    private static List<TurnInstruction>? MapActions(HereRouteSection section)
    {
        // HERE v8 returns actions in "turnByTurnActions" or "actions" depending on return param
        var actions = section.TurnByTurnActions ?? section.Actions;
        if (actions is null or { Count: 0 })
            return null;

        return actions.Select(a => new TurnInstruction
        {
            Action = a.Action,
            Instruction = a.Instruction,
            Duration = a.Duration,
            Length = a.Length,
            Offset = a.Offset
        }).ToList();
    }

    private static void AddTruckParameters(List<(string key, string? value)> parameters, TruckOptions truck)
    {
        if (truck.Height.HasValue)
            parameters.Add(("truck[height]", HereApiHelper.Invariant((int)(truck.Height.Value * 100))));
        if (truck.Width.HasValue)
            parameters.Add(("truck[width]", HereApiHelper.Invariant((int)(truck.Width.Value * 100))));
        if (truck.Length.HasValue)
            parameters.Add(("truck[length]", HereApiHelper.Invariant((int)(truck.Length.Value * 100))));
        if (truck.GrossWeight.HasValue)
            parameters.Add(("truck[grossWeight]", HereApiHelper.Invariant(truck.GrossWeight.Value)));
        if (truck.WeightPerAxle.HasValue)
            parameters.Add(("truck[weightPerAxle]", HereApiHelper.Invariant(truck.WeightPerAxle.Value)));
        if (truck.AxleCount.HasValue)
            parameters.Add(("truck[axleCount]", truck.AxleCount.Value.ToString()));
        if (truck.TrailerCount.HasValue)
            parameters.Add(("truck[trailerCount]", truck.TrailerCount.Value.ToString()));
        if (truck.TunnelCategory.HasValue)
            parameters.Add(("truck[tunnelCategory]", HereApiHelper.GetEnumMemberValue(truck.TunnelCategory.Value)));
        if (truck.HazardousGoods != HazardousGoods.None)
        {
            var goods = HereApiHelper.GetHazardousGoods(truck.HazardousGoods);
            if (goods.Length > 0)
                parameters.Add(("truck[shippedHazardousGoods]", string.Join(",", goods)));
        }
    }

    private static void AddEvParameters(List<(string key, string? value)> parameters, EvOptions ev)
    {
        if (ev.InitialCharge.HasValue)
            parameters.Add(("ev[initialCharge]", HereApiHelper.Invariant(ev.InitialCharge.Value)));
        if (ev.MaxCharge.HasValue)
            parameters.Add(("ev[maxCharge]", HereApiHelper.Invariant(ev.MaxCharge.Value)));
        if (ev.MaxChargeAfterChargingStation.HasValue)
            parameters.Add(("ev[maxChargeAfterChargingStation]", HereApiHelper.Invariant(ev.MaxChargeAfterChargingStation.Value)));
        if (ev.MinChargeAtChargingStation.HasValue)
            parameters.Add(("ev[minChargeAtChargingStation]", HereApiHelper.Invariant(ev.MinChargeAtChargingStation.Value)));
        if (ev.MinChargeAtDestination.HasValue)
            parameters.Add(("ev[minChargeAtDestination]", HereApiHelper.Invariant(ev.MinChargeAtDestination.Value)));
        if (ev.ChargingCurve is not null)
            parameters.Add(("ev[chargingCurve]", ev.ChargingCurve));
        if (ev.FreeFlowSpeedTable is not null)
            parameters.Add(("ev[freeFlowSpeedTable]", ev.FreeFlowSpeedTable));
        if (ev.AuxiliaryConsumption.HasValue)
            parameters.Add(("ev[auxiliaryConsumption]", HereApiHelper.Invariant(ev.AuxiliaryConsumption.Value)));
    }
}
