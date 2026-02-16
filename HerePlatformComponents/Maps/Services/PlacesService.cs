using HerePlatform.Core.Places;
using HerePlatform.Core.Services;
using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of IPlacesService using HERE JS API's H.service.SearchService.
/// </summary>
public class PlacesService : IPlacesService
{
    private readonly IJSRuntime _js;

    public PlacesService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<PlacesResult> DiscoverAsync(PlacesRequest request)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        var result = await _js.InvokeAsync<PlacesResult>(
            JsInteropIdentifiers.DiscoverPlaces,
            cts.Token,
            request);

        return result ?? new PlacesResult();
    }

    public async Task<PlacesResult> BrowseAsync(PlacesRequest request)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        var result = await _js.InvokeAsync<PlacesResult>(
            JsInteropIdentifiers.BrowsePlaces,
            cts.Token,
            request);

        return result ?? new PlacesResult();
    }

    public async Task<PlacesResult> LookupAsync(PlacesRequest request)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        var result = await _js.InvokeAsync<PlacesResult>(
            JsInteropIdentifiers.LookupPlace,
            cts.Token,
            request);

        return result ?? new PlacesResult();
    }
}
