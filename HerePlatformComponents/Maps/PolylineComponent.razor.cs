using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class PolylineComponent : MapObjectComponentBase
{
    /// <summary>
    /// Path defining the polyline. Two-way bindable via <c>@bind-Path</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public List<LatLngLiteral>? Path { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<List<LatLngLiteral>?> PathChanged { get; set; }

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
    /// Line join style: "round", "miter", "bevel".
    /// </summary>
    [Parameter, JsonIgnore]
    public string? LineJoin { get; set; }

    /// <summary>
    /// Offset into the dash pattern.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? LineDashOffset { get; set; }

    /// <summary>
    /// If true, renders directional arrows on the polyline.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool? Arrows { get; set; }

    /// <summary>
    /// Z-index for stacking order.
    /// </summary>
    [Parameter, JsonIgnore]
    public int? ZIndex { get; set; }

    /// <summary>
    /// 3D extrusion height in meters (HARP engine).
    /// </summary>
    [Parameter, JsonIgnore]
    public double? Extrusion { get; set; }

    /// <summary>
    /// 3D base elevation in meters (HARP engine).
    /// </summary>
    [Parameter, JsonIgnore]
    public double? Elevation { get; set; }

    /// <summary>
    /// If true, the polyline can be dragged.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Draggable { get; set; }

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

    internal async Task HandleGeometryChanged(List<LatLngLiteral> path)
    {
        Path = path;

        if (PathChanged.HasDelegate)
            await PathChanged.InvokeAsync(path);
    }

    internal override bool HasAnyEventCallback =>
        HasBaseEventCallbacks || PathChanged.HasDelegate;

    protected override string JsDisposeFunction => "blazorHerePlatform.objectManager.disposePolylineComponent";

    protected override Task RegisterWithMapAsync() => MapRef.AddPolyline(this);

    protected override Task UnregisterFromMapAsync()
    {
        if (MapRef is not null)
            return MapRef.RemovePolyline(this);
        return Task.CompletedTask;
    }

    protected override async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updatePolylineComponent",
            [Guid,
            new PolylineComponentOptions
            {
                Path = Path,
                StrokeColor = StrokeColor,
                LineWidth = LineWidth,
                LineCap = LineCap,
                LineDash = LineDash,
                LineJoin = LineJoin,
                LineDashOffset = LineDashOffset,
                Arrows = Arrows,
                ZIndex = ZIndex,
                Extrusion = Extrusion,
                Elevation = Elevation,
                Draggable = Draggable,
                Clickable = Clickable || Draggable || HasAnyEventCallback,
                Visible = Visible,
                MapId = MapRef.MapId,
            },
            MapRef.CallbackRef]);
    }

    protected override bool CheckParameterChanges(ParameterView parameters)
    {
        return parameters.DidParameterChange(Path) ||
            parameters.DidParameterChange(StrokeColor) ||
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(LineCap) ||
            parameters.DidParameterChange(LineDash) ||
            parameters.DidParameterChange(LineJoin) ||
            parameters.DidParameterChange(LineDashOffset) ||
            parameters.DidParameterChange(Arrows) ||
            parameters.DidParameterChange(ZIndex) ||
            parameters.DidParameterChange(Extrusion) ||
            parameters.DidParameterChange(Elevation) ||
            parameters.DidParameterChange(Draggable) ||
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Visible);
    }

    internal readonly struct PolylineComponentOptions
    {
        public List<LatLngLiteral>? Path { get; init; }
        public string? StrokeColor { get; init; }
        public double? LineWidth { get; init; }
        public string? LineCap { get; init; }
        public double[]? LineDash { get; init; }
        public string? LineJoin { get; init; }
        public double? LineDashOffset { get; init; }
        public bool? Arrows { get; init; }
        public int? ZIndex { get; init; }
        public double? Extrusion { get; init; }
        public double? Elevation { get; init; }
        public bool Draggable { get; init; }
        public bool? Clickable { get; init; }
        public bool? Visible { get; init; }
        public Guid? MapId { get; init; }
    }
}
