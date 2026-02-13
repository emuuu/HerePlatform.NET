using HerePlatformComponents.Maps.Events;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// DomMarker component that renders a marker using HTML (DomIcon).
/// ChildContent provides the HTML template for the DomIcon.
/// Note: ChildContent is read as static HTML — no live Blazor interactivity inside the DomIcon.
/// </summary>
public partial class DomMarkerComponent : IAsyncDisposable
{
    public DomMarkerComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    internal bool IsDisposed = false;
    private Guid _guid;

    public Guid Guid => _guid;

    internal string TemplateElementId => $"blz-dm-{_guid}";

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    [CascadingParameter(Name = "HereGroup")]
    private GroupComponent? GroupRef { get; set; }

    /// <summary>
    /// HTML content for the DomIcon. Read as static HTML.
    /// </summary>
    [Parameter, JsonIgnore]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Latitude in degrees. Two-way bindable via <c>@bind-Lat</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lat { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> LatChanged { get; set; }

    /// <summary>
    /// Longitude in degrees. Two-way bindable via <c>@bind-Lng</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lng { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> LngChanged { get; set; }

    /// <summary>
    /// If true, the marker can be clicked.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Clickable { get; set; }

    /// <summary>
    /// If true, the marker can be dragged.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Draggable { get; set; }

    /// <summary>
    /// Opacity of the marker (0-1).
    /// </summary>
    [Parameter, JsonIgnore]
    public double? Opacity { get; set; }

    /// <summary>
    /// Minimum zoom level at which the marker is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom level at which the marker is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? MaxZoom { get; set; }

    /// <summary>
    /// Whether the marker is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Z-index for stacking order.
    /// </summary>
    [Parameter, JsonIgnore]
    public int? ZIndex { get; set; }

    /// <summary>
    /// Arbitrary data associated with this DomMarker.
    /// </summary>
    [Parameter, JsonIgnore]
    public object? Data { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnClick { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnDoubleClick { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnLongPress { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnContextMenu { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback OnContextMenuClose { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerDown { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerUp { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerMove { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerEnter { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerLeave { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback OnPointerCancel { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDragStart { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDrag { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDragEnd { get; set; }

    internal async Task HandlePointerEvent(string eventName, MapPointerEventArgs args)
    {
        var callback = eventName switch
        {
            "tap" => OnClick,
            "dbltap" => OnDoubleClick,
            "longpress" => OnLongPress,
            "contextmenu" => OnContextMenu,
            "pointerdown" => OnPointerDown,
            "pointerup" => OnPointerUp,
            "pointermove" => OnPointerMove,
            "pointerenter" => OnPointerEnter,
            "pointerleave" => OnPointerLeave,
            _ => default
        };

        if (callback.HasDelegate)
            await callback.InvokeAsync(args);
    }

    internal async Task HandleContextMenuClose()
    {
        if (OnContextMenuClose.HasDelegate)
            await OnContextMenuClose.InvokeAsync();
    }

    internal async Task HandlePointerCancel()
    {
        if (OnPointerCancel.HasDelegate)
            await OnPointerCancel.InvokeAsync();
    }

    internal async Task HandleDragEvent(string eventName, MapDragEventArgs args)
    {
        if (eventName == "dragend" && args.Position.HasValue)
        {
            var newLat = args.Position.Value.Lat;
            var newLng = args.Position.Value.Lng;

            Lat = newLat;
            Lng = newLng;

            if (LatChanged.HasDelegate)
                await LatChanged.InvokeAsync(newLat);
            if (LngChanged.HasDelegate)
                await LngChanged.InvokeAsync(newLng);
        }

        var callback = eventName switch
        {
            "dragstart" => OnDragStart,
            "drag" => OnDrag,
            "dragend" => OnDragEnd,
            _ => default
        };

        if (callback.HasDelegate)
            await callback.InvokeAsync(args);
    }

    internal bool HasAnyEventCallback =>
        OnClick.HasDelegate || OnDoubleClick.HasDelegate || OnLongPress.HasDelegate ||
        OnContextMenu.HasDelegate || OnContextMenuClose.HasDelegate ||
        OnPointerDown.HasDelegate || OnPointerUp.HasDelegate || OnPointerMove.HasDelegate ||
        OnPointerEnter.HasDelegate || OnPointerLeave.HasDelegate || OnPointerCancel.HasDelegate ||
        OnDragStart.HasDelegate || OnDrag.HasDelegate || OnDragEnd.HasDelegate ||
        LatChanged.HasDelegate || LngChanged.HasDelegate;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MapRef.AddDomMarker(this);
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateDomMarkerComponent",
            Guid,
            new DomMarkerComponentOptions
            {
                Position = new LatLngLiteral(Lat, Lng),
                Clickable = Clickable || Draggable || HasAnyEventCallback,
                Draggable = Draggable,
                ZIndex = ZIndex,
                Opacity = Opacity,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                Visible = Visible,
                MapId = MapRef.MapId,
                GroupId = GroupRef?.Guid,
                TemplateId = ChildContent is not null ? TemplateElementId : null,
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
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Draggable) ||
            parameters.DidParameterChange(ZIndex) ||
            parameters.DidParameterChange(Opacity) ||
            parameters.DidParameterChange(MinZoom) ||
            parameters.DidParameterChange(MaxZoom) ||
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
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeDomMarkerComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        MapRef?.RemoveDomMarker(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct DomMarkerComponentOptions
    {
        public LatLngLiteral? Position { get; init; }
        public bool Clickable { get; init; }
        public bool Draggable { get; init; }
        public int? ZIndex { get; init; }
        public double? Opacity { get; init; }
        public double? MinZoom { get; init; }
        public double? MaxZoom { get; init; }
        public bool Visible { get; init; }
        public Guid? MapId { get; init; }
        public Guid? GroupId { get; init; }
        public string? TemplateId { get; init; }
    }
}
