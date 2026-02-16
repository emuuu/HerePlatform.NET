using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Services;
using HerePlatform.Core.Transit;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of IPublicTransitService using HERE JS API.
/// </summary>
public class PublicTransitService : IPublicTransitService
{
    private readonly IJSRuntime _js;

    public PublicTransitService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<TransitDeparturesResult> GetDeparturesAsync(LatLngLiteral position)
    {
        var result = await _js.InvokeAsync<TransitDeparturesResult>(
            JsInteropIdentifiers.GetTransitDepartures,
            position);

        return result ?? new TransitDeparturesResult();
    }

    public async Task<TransitStationsResult> SearchStationsAsync(LatLngLiteral position, double radiusMeters = 500)
    {
        var result = await _js.InvokeAsync<TransitStationsResult>(
            JsInteropIdentifiers.SearchTransitStations,
            position, radiusMeters);

        return result ?? new TransitStationsResult();
    }
}
