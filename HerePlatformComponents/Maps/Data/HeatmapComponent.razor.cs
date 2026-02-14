using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Data;

/// <summary>
/// Heatmap visualization component.
/// </summary>
public partial class HeatmapComponent : IAsyncDisposable
{
    public HeatmapComponent()
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
    /// Data points for the heatmap.
    /// </summary>
    [Parameter, JsonIgnore]
    public List<HeatmapDataPoint>? DataPoints { get; set; }

    /// <summary>
    /// Opacity of the heatmap layer (0-1).
    /// </summary>
    [Parameter, JsonIgnore]
    public double Opacity { get; set; } = 0.6;

    /// <summary>
    /// Color stops for the heatmap gradient. Keys are 0-1 thresholds, values are CSS colors.
    /// </summary>
    [Parameter, JsonIgnore]
    public Dictionary<double, string>? Colors { get; set; }

    /// <summary>
    /// Sampling depth for heatmap calculation.
    /// </summary>
    [Parameter, JsonIgnore]
    public int? SampleDepth { get; set; }

    /// <summary>
    /// If true, the heatmap layer is visible.
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
            "blazorHerePlatform.objectManager.updateHeatmapComponent",
            Guid,
            new
            {
                dataPoints = DataPoints,
                opacity = Opacity,
                colors = Colors,
                sampleDepth = SampleDepth,
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
            parameters.DidParameterChange(DataPoints) ||
            parameters.DidParameterChange(Opacity) ||
            parameters.DidParameterChange(Colors) ||
            parameters.DidParameterChange(SampleDepth) ||
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
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeHeatmapComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        GC.SuppressFinalize(this);
    }
}
