using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class PolygonComponent : MapObjectComponentBase
{
    /// <summary>
    /// Path defining the polygon boundary. Two-way bindable via <c>@bind-Path</c>.
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

    /// <summary>
    /// Arbitrary data associated with this polygon.
    /// </summary>
    [Parameter, JsonIgnore]
    public object? Data { get; set; }

    /// <summary>
    /// Interior rings (holes) for the polygon. Each hole is a list of LatLngLiteral.
    /// </summary>
    [Parameter, JsonIgnore]
    public List<List<LatLngLiteral>>? Holes { get; set; }

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
    /// If true, the polygon can be dragged.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Draggable { get; set; }

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

    internal async Task HandleGeometryChanged(List<LatLngLiteral> path)
    {
        Path = path;

        if (PathChanged.HasDelegate)
            await PathChanged.InvokeAsync(path);
    }

    internal override bool HasAnyEventCallback =>
        HasBaseEventCallbacks || PathChanged.HasDelegate;

    protected override string JsDisposeFunction => JsInteropIdentifiers.DisposePolygonComponent;

    protected override Task RegisterWithMapAsync() => MapRef.AddPolygon(this);

    protected override Task UnregisterFromMapAsync()
    {
        if (MapRef is not null)
            return MapRef.RemovePolygon(this);
        return Task.CompletedTask;
    }

    protected override async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            JsInteropIdentifiers.UpdatePolygonComponent,
            [Guid,
            new PolygonComponentOptions
            {
                Path = Path,
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
                Holes = Holes,
                Extrusion = Extrusion,
                Elevation = Elevation,
                MapId = MapRef.MapId,
            },
            MapRef.CallbackRef]);
    }

    protected override bool CheckParameterChanges(ParameterView parameters)
    {
        return parameters.DidParameterChange(Path) ||
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
            parameters.DidParameterChange(Visible) ||
            parameters.DidParameterChange(Holes) ||
            parameters.DidParameterChange(Extrusion) ||
            parameters.DidParameterChange(Elevation);
    }

    internal readonly struct PolygonComponentOptions
    {
        public List<LatLngLiteral>? Path { get; init; }
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
        public List<List<LatLngLiteral>>? Holes { get; init; }
        public double? Extrusion { get; init; }
        public double? Elevation { get; init; }
        public Guid? MapId { get; init; }
    }
}
