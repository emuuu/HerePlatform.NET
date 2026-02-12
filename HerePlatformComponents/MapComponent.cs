using HerePlatformComponents.Maps;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HerePlatformComponents;

public class MapComponent : ComponentBase, IDisposable, IAsyncDisposable
{
    private bool _isDisposed;

    [Inject]
    public IJSRuntime JsRuntime { get; protected set; } = default!;

    [Inject]
    public IServiceProvider ServiceProvider { get; protected set; } = default!;

    private IBlazorHerePlatformKeyService? _keyService;
    internal event EventHandler? MapInitialized;

    protected override void OnInitialized()
    {
        _keyService = ServiceProvider.GetService<IBlazorHerePlatformKeyService>();
        base.OnInitialized();
    }

    public Map InteropObject { get; private set; } = default!;

    public async Task InitAsync(ElementReference element, MapOptions? options = null)
    {
        HereApiLoadOptions? loadedApiOptions = options?.ApiLoadOptions;
        if (options?.ApiLoadOptions == null && _keyService != null)
        {
            bool isHereReady = false;
            try
            {
                isHereReady = await JsRuntime.InvokeAsync<bool>("blazorHerePlatform.objectManager.isHereMapsReady");
            }
            catch
            {
                // Ignore JS exceptions; we'll try loading if needed.
            }

            if (!_keyService.IsApiInitialized || !isHereReady)
            {
                _keyService.IsApiInitialized = true;
                options ??= new MapOptions();
                loadedApiOptions = await _keyService.GetApiOptions();
                options.ApiLoadOptions = loadedApiOptions;
            }
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
            catch { }
            InteropObject = null!;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (InteropObject is not null)
        {
            try
            {
                await InteropObject.DisposeAsync();
                InteropObject = null!;
            }
            catch (Exception ex)
            {
                var isPossibleRefreshError = ex.HasInnerExceptionsOfType<TaskCanceledException>();
                isPossibleRefreshError |= ex.HasInnerExceptionsOfType<ObjectDisposedException>();
                isPossibleRefreshError |= true;

                if (!isPossibleRefreshError)
                {
                    throw;
                }
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                InteropObject?.Dispose();
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
