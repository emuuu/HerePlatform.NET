using HerePlatformComponents.Maps.Events;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class PolygonComponent : IAsyncDisposable
{
    public PolygonComponent()
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
    /// Path defining the polygon boundary.
    /// </summary>
    [Parameter, JsonIgnore]
    public List<LatLngLiteral>? Path { get; set; }

    /// <summary>
    /// Stroke color in CSS format.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? StrokeColor { get; set; }

    /// <summary>
    /// Fill color in CSS format.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? FillColor { get; set; }

    /// <summary>
    /// Line width in pixels.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? LineWidth { get; set; }

    /// <summary>
    /// If true, the polygon is clickable.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Clickable { get; set; }

    /// <summary>
    /// If true, the polygon is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    #region Pointer / Interaction EventCallbacks

    /// <summary>
    /// Fired when the polygon is tapped/clicked.
    /// Breaking change: now provides MapPointerEventArgs.
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
    /// Fired when a pointer touches the polygon surface.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerDown { get; set; }

    /// <summary>
    /// Fired when a pointer leaves the polygon surface.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerUp { get; set; }

    /// <summary>
    /// Fired when a pointer moves over the polygon.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerMove { get; set; }

    /// <summary>
    /// Fired when a pointer enters the polygon area.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerEnter { get; set; }

    /// <summary>
    /// Fired when a pointer leaves the polygon area.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerLeave { get; set; }

    /// <summary>
    /// Fired when a pointer action is cancelled.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback OnPointerCancel { get; set; }

    /// <summary>
    /// Fired when a drag operation starts on the polygon.
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
            MapRef.AddPolygon(this);
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updatePolygonComponent",
            Guid,
            new PolygonComponentOptions
            {
                Path = Path,
                StrokeColor = StrokeColor,
                FillColor = FillColor,
                LineWidth = LineWidth,
                Clickable = Clickable || HasAnyEventCallback,
                Visible = Visible,
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
            parameters.DidParameterChange(Path) ||
            parameters.DidParameterChange(StrokeColor) ||
            parameters.DidParameterChange(FillColor) ||
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(Clickable) ||
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
        await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposePolygonComponent", Guid);
        MapRef.RemovePolygon(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct PolygonComponentOptions
    {
        public List<LatLngLiteral>? Path { get; init; }
        public string? StrokeColor { get; init; }
        public string? FillColor { get; init; }
        public double? LineWidth { get; init; }
        public bool? Clickable { get; init; }
        public bool? Visible { get; init; }
        public Guid? MapId { get; init; }
    }
}
