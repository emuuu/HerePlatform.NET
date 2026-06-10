using HerePlatform.Blazor.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatform.Blazor.Maps.UI;

/// <summary>
/// Overview (mini-map) UI control.
/// </summary>
public partial class OverviewMapComponent : IAsyncDisposable
{
    public OverviewMapComponent()
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
    /// UI alignment position.
    /// </summary>
    [Parameter, JsonIgnore]
    public UIAlignment Alignment { get; set; } = UIAlignment.BottomRight;

    /// <summary>
    /// Zoom level delta between main map and overview (default: 4).
    /// </summary>
    [Parameter, JsonIgnore]
    public int ZoomDelta { get; set; } = 4;

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
            JsInteropIdentifiers.UpdateOverviewMap,
            Guid,
            new
            {
                alignment = Helper.GetEnumMemberValue(Alignment) ?? "bottom-right",
                zoomDelta = ZoomDelta,
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
            parameters.DidParameterChange(Alignment) ||
            parameters.DidParameterChange(ZoomDelta);

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
            MapRef?.UnregisterAuxComponent(_guid);
            GC.SuppressFinalize(this);
            return;
        }
        _isDisposed = true;

        MapRef?.UnregisterAuxComponent(_guid);

        try
        {
            await Js.InvokeVoidAsync(JsInteropIdentifiers.DisposeOverviewMap, Guid);
        }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }

        GC.SuppressFinalize(this);
    }
}
