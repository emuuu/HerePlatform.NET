using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Custom tile layer component for overlaying custom tile sources on the map.
/// </summary>
public partial class CustomTileLayerComponent : IAsyncDisposable
{
    public CustomTileLayerComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    private bool _isDisposed;
    internal void MarkDisposed() => _isDisposed = true;
    private Guid _guid;

    public Guid Guid => _guid;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// URL pattern with {x}, {y}, {z} placeholders.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? UrlPattern { get; set; }

    /// <summary>
    /// Minimum zoom level.
    /// </summary>
    [Parameter, JsonIgnore]
    public int Min { get; set; } = 0;

    /// <summary>
    /// Maximum zoom level.
    /// </summary>
    [Parameter, JsonIgnore]
    public int Max { get; set; } = 20;

    /// <summary>
    /// Tile size in pixels.
    /// </summary>
    [Parameter, JsonIgnore]
    public int TileSize { get; set; } = 256;

    /// <summary>
    /// Layer opacity (0-1).
    /// </summary>
    [Parameter, JsonIgnore]
    public double Opacity { get; set; } = 1.0;

    /// <summary>
    /// If true, the custom tile layer is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateCustomTileLayer",
            Guid,
            new
            {
                urlPattern = UrlPattern,
                min = Min,
                max = Max,
                tileSize = TileSize,
                opacity = Opacity,
                visible = Visible,
                mapId = MapRef.MapId
            },
            MapRef.CallbackRef);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (!_hasRendered)
        {
            await base.SetParametersAsync(parameters);
            return;
        }

        var optionsChanged =
            parameters.DidParameterChange(UrlPattern) ||
            parameters.DidParameterChange(Min) ||
            parameters.DidParameterChange(Max) ||
            parameters.DidParameterChange(TileSize) ||
            parameters.DidParameterChange(Opacity) ||
            parameters.DidParameterChange(Visible);

        await base.SetParametersAsync(parameters);

        if (optionsChanged)
        {
            await UpdateOptions();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeCustomTileLayer", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        GC.SuppressFinalize(this);
    }
}
