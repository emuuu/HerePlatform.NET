using HerePlatformComponents.Maps.Events;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class PolylineComponent : IAsyncDisposable
{
    public PolylineComponent()
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
    /// Path defining the polyline.
    /// </summary>
    [Parameter, JsonIgnore]
    public List<LatLngLiteral>? Path { get; set; }

    /// <summary>
    /// Stroke color in CSS format.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? StrokeColor { get; set; }

    /// <summary>
    /// Line width in pixels.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? LineWidth { get; set; }

    /// <summary>
    /// Line cap style: "round", "square", "butt".
    /// </summary>
    [Parameter, JsonIgnore]
    public string? LineCap { get; set; }

    /// <summary>
    /// Array of dash pattern [dash, gap, dash, gap, ...].
    /// </summary>
    [Parameter, JsonIgnore]
    public double[]? LineDash { get; set; }

    /// <summary>
    /// If true, the polyline is clickable.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Clickable { get; set; }

    /// <summary>
    /// If true, the polyline is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Arbitrary data associated with this polyline.
    /// </summary>
    [Parameter, JsonIgnore]
    public object? Data { get; set; }

    #region Pointer / Interaction EventCallbacks

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
            MapRef.AddPolyline(this);
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updatePolylineComponent",
            Guid,
            new PolylineComponentOptions
            {
                Path = Path,
                StrokeColor = StrokeColor,
                LineWidth = LineWidth,
                LineCap = LineCap,
                LineDash = LineDash,
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
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(LineCap) ||
            parameters.DidParameterChange(LineDash) ||
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

        try
        {
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposePolylineComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        MapRef.RemovePolyline(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct PolylineComponentOptions
    {
        public List<LatLngLiteral>? Path { get; init; }
        public string? StrokeColor { get; init; }
        public double? LineWidth { get; init; }
        public string? LineCap { get; init; }
        public double[]? LineDash { get; init; }
        public bool? Clickable { get; init; }
        public bool? Visible { get; init; }
        public Guid? MapId { get; init; }
    }
}
