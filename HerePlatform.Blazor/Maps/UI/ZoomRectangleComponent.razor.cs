using HerePlatform.Blazor.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatform.Blazor.Maps.UI;

/// <summary>
/// Zoom rectangle UI control allowing users to draw a rectangle to zoom into.
/// </summary>
public partial class ZoomRectangleComponent : IAsyncDisposable
{
    public ZoomRectangleComponent()
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
    /// Whether the zoom rectangle tool is active.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Active { get; set; }

    /// <summary>
    /// UI alignment position.
    /// </summary>
    [Parameter, JsonIgnore]
    public UIAlignment Alignment { get; set; } = UIAlignment.TopRight;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MapRef?.RegisterAuxComponent(_guid, MarkDisposed);
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            JsInteropIdentifiers.UpdateZoomRectangle,
            Guid,
            new
            {
                active = Active,
                alignment = Helper.GetEnumMemberValue(Alignment) ?? "top-right",
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
            parameters.DidParameterChange(Active) ||
            parameters.DidParameterChange(Alignment);

        await base.SetParametersAsync(parameters);

        if (optionsChanged)
        {
            await UpdateOptions();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            // Parent map already marked us. Skip JS call.
            MapRef?.UnregisterAuxComponent(_guid);
            GC.SuppressFinalize(this);
            return;
        }
        _isDisposed = true;

        MapRef?.UnregisterAuxComponent(_guid);

        try
        {
            await Js.InvokeVoidAsync(JsInteropIdentifiers.DisposeZoomRectangle, Guid);
        }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }

        GC.SuppressFinalize(this);
    }
}
