using HerePlatform.Core.MatrixRouting;
using HerePlatform.Core.Services;
using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Implementation of IMatrixRoutingService using HERE Matrix Routing API v8 via REST.
/// </summary>
public class MatrixRoutingService : IMatrixRoutingService
{
    private readonly IJSRuntime _js;

    public MatrixRoutingService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<MatrixRoutingResult> CalculateMatrixAsync(MatrixRoutingRequest request, CancellationToken cancellationToken = default)
    {
        MatrixRoutingResult? result;
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            result = await _js.InvokeAsync<MatrixRoutingResult>(
                JsInteropIdentifiers.CalculateMatrix,
                cts.Token,
                request);
        }
        catch (JSException ex)
        {
            JsAuthErrorHelper.ThrowIfAuthError(ex, "matrix-routing");
            throw;
        }

        return result ?? new MatrixRoutingResult();
    }
}
