using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class MarkerComponent : IAsyncDisposable
{
    public MarkerComponent()
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

    [Parameter, JsonIgnore]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Latitude in degrees.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lat { get; set; }

    /// <summary>
    /// Longitude in degrees.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lng { get; set; }

    /// <summary>
    /// If true, the marker can be clicked and triggers tap event.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Clickable { get; set; }

    /// <summary>
    /// If true, the marker can be dragged.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Draggable { get; set; }

    /// <summary>
    /// Title/tooltip text for accessibility.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? Title { get; set; }

    /// <summary>
    /// Z-index for stacking order.
    /// </summary>
    [Parameter, JsonIgnore]
    public int? ZIndex { get; set; }

    /// <summary>
    /// Whether the marker is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// URL or SVG string for a custom icon.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? IconUrl { get; set; }

    /// <summary>
    /// Fired when the marker is tapped/clicked.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback OnClick { get; set; }

    /// <summary>
    /// Fired when the marker is dragged to a new position.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<LatLngLiteral> OnMove { get; set; }

    internal async Task MarkerClicked()
    {
        await OnClick.InvokeAsync();
    }

    internal async Task MarkerDragged(LatLngLiteral position)
    {
        await OnMove.InvokeAsync(position);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MapRef.AddMarker(this);
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task ForceRender()
    {
        if (!_hasRendered) return;
        await UpdateOptions();
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateMarkerComponent",
            Guid,
            new MarkerComponentOptions
            {
                Position = new LatLngLiteral(Lat, Lng),
                Draggable = Draggable,
                Clickable = Clickable || Draggable,
                ZIndex = ZIndex,
                Visible = Visible,
                IconUrl = IconUrl,
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
            parameters.DidParameterChange(Lat) ||
            parameters.DidParameterChange(Lng) ||
            parameters.DidParameterChange(ZIndex) ||
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Draggable) ||
            parameters.DidParameterChange(Visible) ||
            parameters.DidParameterChange(IconUrl);

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
        await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeMarkerComponent", Guid);
        MapRef.RemoveMarker(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct MarkerComponentOptions
    {
        public LatLngLiteral? Position { get; init; }
        public Guid? MapId { get; init; }
        public bool Draggable { get; init; }
        public bool Clickable { get; init; }
        public int? ZIndex { get; init; }
        public bool Visible { get; init; }
        public string? IconUrl { get; init; }
    }
}
