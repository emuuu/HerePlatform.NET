using HerePlatformComponents.Maps.Events;
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

    #region Pointer / Interaction EventCallbacks

    /// <summary>
    /// Fired when the marker is tapped/clicked.
    /// Breaking change: now provides MapPointerEventArgs with position and viewport data.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnClick { get; set; }

    /// <summary>
    /// Fired on double-tap/double-click.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnDoubleClick { get; set; }

    /// <summary>
    /// Fired on long press.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnLongPress { get; set; }

    /// <summary>
    /// Fired on right-click / long-press context menu.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnContextMenu { get; set; }

    /// <summary>
    /// Fired when the context menu interaction ends.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback OnContextMenuClose { get; set; }

    /// <summary>
    /// Fired when a pointer touches the marker surface.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerDown { get; set; }

    /// <summary>
    /// Fired when a pointer leaves the marker surface.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerUp { get; set; }

    /// <summary>
    /// Fired when a pointer moves over the marker.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerMove { get; set; }

    /// <summary>
    /// Fired when a pointer enters the marker area.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerEnter { get; set; }

    /// <summary>
    /// Fired when a pointer leaves the marker area.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerLeave { get; set; }

    /// <summary>
    /// Fired when a pointer action is cancelled.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback OnPointerCancel { get; set; }

    /// <summary>
    /// Fired when a drag operation starts on the marker.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDragStart { get; set; }

    /// <summary>
    /// Fired continuously during a drag operation.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDrag { get; set; }

    /// <summary>
    /// Fired when a drag operation ends.
    /// Breaking change: replaces OnMove. Now provides MapDragEventArgs.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDragEnd { get; set; }

    #endregion

    #region Internal event handlers (called by AdvancedHereMap)

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

    #endregion

    /// <summary>
    /// Returns true if any pointer/interaction event callback is bound.
    /// Used to determine whether JS should wire up event listeners.
    /// </summary>
    internal bool HasAnyEventCallback =>
        OnClick.HasDelegate || OnDoubleClick.HasDelegate || OnLongPress.HasDelegate ||
        OnContextMenu.HasDelegate || OnContextMenuClose.HasDelegate ||
        OnPointerDown.HasDelegate || OnPointerUp.HasDelegate || OnPointerMove.HasDelegate ||
        OnPointerEnter.HasDelegate || OnPointerLeave.HasDelegate || OnPointerCancel.HasDelegate ||
        OnDragStart.HasDelegate || OnDrag.HasDelegate || OnDragEnd.HasDelegate;

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
                Clickable = Clickable || Draggable || HasAnyEventCallback,
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
