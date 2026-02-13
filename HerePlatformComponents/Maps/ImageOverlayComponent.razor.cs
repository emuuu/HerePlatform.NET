using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Image overlay component for stretching an image (PNG/SVG) over geographic bounds.
/// </summary>
public partial class ImageOverlayComponent : IAsyncDisposable
{
    public ImageOverlayComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    internal bool IsDisposed = false;
    private Guid _guid;

    public Guid Guid => _guid;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// URL of the image to overlay (HTTP URL or data URI).
    /// </summary>
    [Parameter, JsonIgnore]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Top latitude of the overlay bounds.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Top { get; set; }

    /// <summary>
    /// Left longitude of the overlay bounds.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Left { get; set; }

    /// <summary>
    /// Bottom latitude of the overlay bounds.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Bottom { get; set; }

    /// <summary>
    /// Right longitude of the overlay bounds.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Right { get; set; }

    /// <summary>
    /// Overlay opacity (0-1).
    /// </summary>
    [Parameter, JsonIgnore]
    public double Opacity { get; set; } = 1.0;

    /// <summary>
    /// If true, the overlay is visible.
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
            "blazorHerePlatform.objectManager.updateImageOverlayComponent",
            Guid,
            new
            {
                imageUrl = ImageUrl,
                top = Top,
                left = Left,
                bottom = Bottom,
                right = Right,
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
            parameters.DidParameterChange(ImageUrl) ||
            parameters.DidParameterChange(Top) ||
            parameters.DidParameterChange(Left) ||
            parameters.DidParameterChange(Bottom) ||
            parameters.DidParameterChange(Right) ||
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
        if (IsDisposed) return;
        IsDisposed = true;

        try
        {
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeImageOverlayComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        GC.SuppressFinalize(this);
    }
}
