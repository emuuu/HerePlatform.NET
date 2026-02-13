using HerePlatformComponents.Maps.Services.WaypointSequence;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of IWaypointSequenceService using HERE JS API.
/// </summary>
public class WaypointSequenceService : IWaypointSequenceService
{
    private readonly IJSRuntime _js;

    public WaypointSequenceService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<WaypointSequenceResult> OptimizeSequenceAsync(WaypointSequenceRequest request)
    {
        var result = await _js.InvokeAsync<WaypointSequenceResult>(
            "blazorHerePlatform.objectManager.optimizeWaypointSequence",
            request);

        return result ?? new WaypointSequenceResult();
    }
}
