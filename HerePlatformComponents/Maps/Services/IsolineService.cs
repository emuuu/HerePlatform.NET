using HerePlatform.Core.Isoline;
using HerePlatform.Core.Services;
using HerePlatform.Core.Utilities;
using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of IIsolineService using HERE JS API's RoutingService.
/// </summary>
public class IsolineService : IIsolineService
{
    private readonly IJSRuntime _js;

    public IsolineService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<IsolineResult> CalculateIsolineAsync(IsolineRequest request, CancellationToken cancellationToken = default)
    {
        IsolineResult? result;
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            result = await _js.InvokeAsync<IsolineResult>(
                JsInteropIdentifiers.CalculateIsoline,
                cts.Token,
                request);
        }
        catch (JSException ex)
        {
            JsAuthErrorHelper.ThrowIfAuthError(ex, "isoline");
            throw;
        }

        // Fallback: decode polylines in C# if JS decoding failed
        if (result?.Isolines != null)
        {
            foreach (var isoline in result.Isolines)
            {
                if ((isoline.Polygon == null || isoline.Polygon.Count == 0)
                    && !string.IsNullOrEmpty(isoline.EncodedPolyline))
                {
                    isoline.Polygon = FlexiblePolyline.Decode(isoline.EncodedPolyline);
                }
            }
        }

        return result ?? new IsolineResult();
    }
}
