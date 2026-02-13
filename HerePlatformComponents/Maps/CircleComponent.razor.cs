using HerePlatformComponents.Maps.Events;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class CircleComponent : IAsyncDisposable
{
    public CircleComponent()
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
    /// Center latitude. Two-way bindable via <c>@bind-CenterLat</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double CenterLat { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> CenterLatChanged { get; set; }

    /// <summary>
    /// Center longitude. Two-way bindable via <c>@bind-CenterLng</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double CenterLng { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> CenterLngChanged { get; set; }

    /// <summary>
    /// Radius in meters. Two-way bindable via <c>@bind-Radius</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Radius { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> RadiusChanged { get; set; }

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
    /// Number of points used to approximate the circle (default: 72).
    /// </summary>
    [Parameter, JsonIgnore]
    public int? Precision { get; set; }

    /// <summary>
    /// If true, the circle is clickable.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Clickable { get; set; }

    /// <summary>
    /// If true, the circle is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Arbitrary data associated with this circle.
    /// </summary>
    [Parameter, JsonIgnore]
    public object? Data { get; set; }

    /// <summary>
    /// 3D extrusion height in meters (HARP engine).
    /// </summary>
    [Parameter, JsonIgnore]
    public double? Extrusion { get; set; }

    /// <summary>
    /// 3D elevation in meters (HARP engine).
    /// </summary>
    [Parameter, JsonIgnore]
    public double? Elevation { get; set; }

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
            MapRef.AddCircle(this);
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateCircleComponent",
            Guid,
            new CircleComponentOptions
            {
                CenterLat = CenterLat,
                CenterLng = CenterLng,
                Radius = Radius,
                StrokeColor = StrokeColor,
                FillColor = FillColor,
                LineWidth = LineWidth,
                Precision = Precision,
                Clickable = Clickable || HasAnyEventCallback,
                Visible = Visible,
                Extrusion = Extrusion,
                Elevation = Elevation,
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
            parameters.DidParameterChange(CenterLat) ||
            parameters.DidParameterChange(CenterLng) ||
            parameters.DidParameterChange(Radius) ||
            parameters.DidParameterChange(StrokeColor) ||
            parameters.DidParameterChange(FillColor) ||
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(Precision) ||
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
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeCircleComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        MapRef.RemoveCircle(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct CircleComponentOptions
    {
        public double CenterLat { get; init; }
        public double CenterLng { get; init; }
        public double Radius { get; init; }
        public string? StrokeColor { get; init; }
        public string? FillColor { get; init; }
        public double? LineWidth { get; init; }
        public int? Precision { get; init; }
        public bool? Clickable { get; init; }
        public bool? Visible { get; init; }
        public double? Extrusion { get; init; }
        public double? Elevation { get; init; }
        public Guid? MapId { get; init; }
    }
}
