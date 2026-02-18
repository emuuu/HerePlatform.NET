using HerePlatform.Core.Routing;
using HerePlatform.Core.Services;
using HerePlatform.Core.Utilities;
using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of IRoutingService using HERE JS API's H.service.RoutingService.
/// </summary>
public class RoutingService : IRoutingService
{
    private readonly IJSRuntime _js;

    public RoutingService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<RoutingResult> CalculateRouteAsync(RoutingRequest request, CancellationToken cancellationToken = default)
    {
        RoutingResult? result;
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            result = await _js.InvokeAsync<RoutingResult>(
                JsInteropIdentifiers.CalculateRoute,
                cts.Token,
                request);
        }
        catch (JSException ex)
        {
            JsAuthErrorHelper.ThrowIfAuthError(ex, "routing");
            throw;
        }

        // Decode polylines
        if (result?.Routes != null)
        {
            foreach (var route in result.Routes)
            {
                if (route.Sections == null) continue;
                foreach (var section in route.Sections)
                {
                    if (!string.IsNullOrEmpty(section.Polyline))
                    {
                        section.DecodedPolyline = FlexiblePolyline.Decode(section.Polyline);
                    }
                }
            }
        }

        return result ?? new RoutingResult();
    }
}
