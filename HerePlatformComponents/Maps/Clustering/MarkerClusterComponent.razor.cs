using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Clustering;

/// <summary>
/// Declarative marker clustering component.
/// Requires HereApiLoadOptions.LoadClustering = true.
/// </summary>
public partial class MarkerClusterComponent : IAsyncDisposable
{
    public MarkerClusterComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    private bool _isDisposed = false;
    private Guid _guid;

    public Guid Guid => _guid;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// Data points to cluster.
    /// </summary>
    [Parameter, JsonIgnore]
    public List<ClusterDataPoint>? DataPoints { get; set; }

    /// <summary>
    /// Epsilon (radius) for clustering. Default: 32.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Eps { get; set; } = 32;

    /// <summary>
    /// Minimum weight to form a cluster. Default: 2.
    /// </summary>
    [Parameter, JsonIgnore]
    public int MinWeight { get; set; } = 2;

    /// <summary>
    /// SVG template for cluster markers. Use {count} and {color} as placeholders.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? ClusterSvgTemplate { get; set; }

    /// <summary>
    /// SVG template for noise (single) markers.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? NoiseSvgTemplate { get; set; }

    /// <summary>
    /// Minimum zoom level at which clustering is active.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom level at which clustering is active.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? MaxZoom { get; set; }

    /// <summary>
    /// Fired when a cluster is tapped.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<ClusterTapEventArgs> OnClusterTap { get; set; }

    /// <summary>
    /// Fired when a noise (single) point is tapped.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<ClusterTapEventArgs> OnNoiseTap { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _hasRendered = true;
            MapRef.AddCluster(this);
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    internal async Task HandleClusterTap(ClusterTapEventArgs args)
    {
        if (OnClusterTap.HasDelegate)
            await OnClusterTap.InvokeAsync(args);
    }

    internal async Task HandleNoiseTap(ClusterTapEventArgs args)
    {
        if (OnNoiseTap.HasDelegate)
            await OnNoiseTap.InvokeAsync(args);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateMarkerClusterComponent",
            Guid,
            new MarkerClusterComponentOptions
            {
                DataPoints = DataPoints,
                Eps = Eps,
                MinWeight = MinWeight,
                ClusterSvgTemplate = ClusterSvgTemplate,
                NoiseSvgTemplate = NoiseSvgTemplate,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                MapId = MapRef.MapId,
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
            parameters.DidParameterChange(Eps) ||
            parameters.DidParameterChange(MinWeight) ||
            parameters.DidParameterChange(ClusterSvgTemplate) ||
            parameters.DidParameterChange(NoiseSvgTemplate) ||
            parameters.DidParameterChange(MinZoom) ||
            parameters.DidParameterChange(MaxZoom);

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

        MapRef?.RemoveCluster(this);

        try
        {
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeMarkerClusterComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        GC.SuppressFinalize(this);
    }

    internal readonly struct MarkerClusterComponentOptions
    {
        public List<ClusterDataPoint>? DataPoints { get; init; }
        public double Eps { get; init; }
        public int MinWeight { get; init; }
        public string? ClusterSvgTemplate { get; init; }
        public string? NoiseSvgTemplate { get; init; }
        public double? MinZoom { get; init; }
        public double? MaxZoom { get; init; }
        public Guid? MapId { get; init; }
    }
}
