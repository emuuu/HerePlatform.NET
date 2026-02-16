using HerePlatform.Core.Services;
using HerePlatform.Core.Traffic;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of ITrafficService using HERE JS API.
/// </summary>
public class TrafficService : ITrafficService
{
    private readonly IJSRuntime _js;

    public TrafficService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<TrafficIncidentsResult> GetTrafficIncidentsAsync(double north, double south, double east, double west)
    {
        var result = await _js.InvokeAsync<TrafficIncidentsResult>(
            JsInteropIdentifiers.GetTrafficIncidents,
            north, south, east, west);

        return result ?? new TrafficIncidentsResult();
    }

    public async Task<TrafficFlowResult> GetTrafficFlowAsync(double north, double south, double east, double west)
    {
        var result = await _js.InvokeAsync<TrafficFlowResult>(
            JsInteropIdentifiers.GetTrafficFlow,
            north, south, east, west);

        return result ?? new TrafficFlowResult();
    }
}
