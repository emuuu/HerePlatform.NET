using HerePlatformComponents.Maps;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatformComponents;

public class MapComponent : ComponentBase, IAsyncDisposable
{
    private bool _isDisposed;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = default!;

    [Inject]
    protected IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private ILogger<MapComponent> Logger { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?>? UserAttributes { get; set; }

    private IBlazorHerePlatformKeyService? _keyService;
    internal event EventHandler? MapInitialized;

    protected override void OnInitialized()
    {
        _keyService = ServiceProvider.GetService<IBlazorHerePlatformKeyService>();
        base.OnInitialized();
    }

    public Map? InteropObject { get; private set; }

    public async Task InitAsync(ElementReference element, MapOptions? options = null)
    {
        if (options?.ApiLoadOptions == null && _keyService != null)
        {
            bool isHereReady = false;
            try
            {
                isHereReady = await JsRuntime.InvokeAsync<bool>("blazorHerePlatform.objectManager.canRenderMap");
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, "canRenderMap check failed");
            }

            if (!_keyService.IsApiInitialized || !isHereReady)
            {
                _keyService.MarkApiInitialized();
            }

            // Always pass ApiLoadOptions so initMap can lazy-load additional modules
            options ??= new MapOptions();
            options.ApiLoadOptions = await _keyService.GetApiOptions();
        }

        InteropObject = await Map.CreateAsync(JsRuntime, element, options);
        MapInitialized?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Disposes only the current map JS object, allowing re-initialization via InitAsync.
    /// </summary>
    protected async Task DisposeMapAsync()
    {
        if (InteropObject is not null)
        {
            try { await InteropObject.DisposeAsync(); }
            catch (Exception ex) { Logger.LogDebug(ex, "Map disposal failed"); }
            InteropObject = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (InteropObject is not null)
        {
            try
            {
                await InteropObject.DisposeAsync();
                InteropObject = null;
            }
            catch (Exception ex)
            {
                var isPossibleRefreshError = ex.HasInnerExceptionsOfType<TaskCanceledException>();
                isPossibleRefreshError |= ex.HasInnerExceptionsOfType<ObjectDisposedException>();

                if (!isPossibleRefreshError)
                {
                    throw;
                }
            }
        }
    }
}
