using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class RectComponent : MapObjectComponentBase
{
    /// <summary>
    /// Top latitude of the bounding rectangle. Two-way bindable via <c>@bind-Top</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Top { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> TopChanged { get; set; }

    /// <summary>
    /// Left longitude of the bounding rectangle. Two-way bindable via <c>@bind-Left</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Left { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> LeftChanged { get; set; }

    /// <summary>
    /// Bottom latitude of the bounding rectangle. Two-way bindable via <c>@bind-Bottom</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Bottom { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> BottomChanged { get; set; }

    /// <summary>
    /// Right longitude of the bounding rectangle. Two-way bindable via <c>@bind-Right</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Right { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<double> RightChanged { get; set; }

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
    /// Line cap style: "round", "square", "butt".
    /// </summary>
    [Parameter, JsonIgnore]
    public string? LineCap { get; set; }

    /// <summary>
    /// Line join style: "round", "miter", "bevel".
    /// </summary>
    [Parameter, JsonIgnore]
    public string? LineJoin { get; set; }

    /// <summary>
    /// Array of dash pattern [dash, gap, dash, gap, ...].
    /// </summary>
    [Parameter, JsonIgnore]
    public double[]? LineDash { get; set; }

    /// <summary>
    /// Offset into the dash pattern.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? LineDashOffset { get; set; }

    /// <summary>
    /// Z-index for stacking order.
    /// </summary>
    [Parameter, JsonIgnore]
    public int? ZIndex { get; set; }

    /// <summary>
    /// If true, the rectangle can be dragged.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Draggable { get; set; }

    /// <summary>
    /// If true, the rectangle is clickable.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Clickable { get; set; }

    /// <summary>
    /// If true, the rectangle is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Arbitrary data associated with this rectangle.
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

    internal async Task HandleGeometryChanged(double top, double left, double bottom, double right)
    {
        Top = top;
        Left = left;
        Bottom = bottom;
        Right = right;

        if (TopChanged.HasDelegate)
            await TopChanged.InvokeAsync(top);
        if (LeftChanged.HasDelegate)
            await LeftChanged.InvokeAsync(left);
        if (BottomChanged.HasDelegate)
            await BottomChanged.InvokeAsync(bottom);
        if (RightChanged.HasDelegate)
            await RightChanged.InvokeAsync(right);
    }

    internal override bool HasAnyEventCallback =>
        HasBaseEventCallbacks ||
        TopChanged.HasDelegate || LeftChanged.HasDelegate || BottomChanged.HasDelegate || RightChanged.HasDelegate;

    protected override string JsDisposeFunction => "blazorHerePlatform.objectManager.disposeRectComponent";

    protected override Task RegisterWithMapAsync() => MapRef.AddRect(this);

    protected override Task UnregisterFromMapAsync()
    {
        if (MapRef is not null)
            return MapRef.RemoveRect(this);
        return Task.CompletedTask;
    }

    protected override async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateRectComponent",
            [Guid,
            new RectComponentOptions
            {
                Top = Top,
                Left = Left,
                Bottom = Bottom,
                Right = Right,
                StrokeColor = StrokeColor,
                FillColor = FillColor,
                LineWidth = LineWidth,
                LineCap = LineCap,
                LineJoin = LineJoin,
                LineDash = LineDash,
                LineDashOffset = LineDashOffset,
                ZIndex = ZIndex,
                Draggable = Draggable,
                Clickable = Clickable || Draggable || HasAnyEventCallback,
                Visible = Visible,
                Extrusion = Extrusion,
                Elevation = Elevation,
                MapId = MapRef.MapId,
            },
            MapRef.CallbackRef]);
    }

    protected override bool CheckParameterChanges(ParameterView parameters)
    {
        return parameters.DidParameterChange(Top) ||
            parameters.DidParameterChange(Left) ||
            parameters.DidParameterChange(Bottom) ||
            parameters.DidParameterChange(Right) ||
            parameters.DidParameterChange(StrokeColor) ||
            parameters.DidParameterChange(FillColor) ||
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(LineCap) ||
            parameters.DidParameterChange(LineJoin) ||
            parameters.DidParameterChange(LineDash) ||
            parameters.DidParameterChange(LineDashOffset) ||
            parameters.DidParameterChange(ZIndex) ||
            parameters.DidParameterChange(Draggable) ||
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Visible);
    }

    internal readonly struct RectComponentOptions
    {
        public double Top { get; init; }
        public double Left { get; init; }
        public double Bottom { get; init; }
        public double Right { get; init; }
        public string? StrokeColor { get; init; }
        public string? FillColor { get; init; }
        public double? LineWidth { get; init; }
        public string? LineCap { get; init; }
        public string? LineJoin { get; init; }
        public double[]? LineDash { get; init; }
        public double? LineDashOffset { get; init; }
        public int? ZIndex { get; init; }
        public bool Draggable { get; init; }
        public bool? Clickable { get; init; }
        public bool? Visible { get; init; }
        public double? Extrusion { get; init; }
        public double? Elevation { get; init; }
        public Guid? MapId { get; init; }
    }
}
