using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class CircleComponent : MapObjectComponentBase
{
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
    /// If true, the circle can be dragged.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Draggable { get; set; }

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

    internal async Task HandleGeometryChanged(double lat, double lng)
    {
        CenterLat = lat;
        CenterLng = lng;

        if (CenterLatChanged.HasDelegate)
            await CenterLatChanged.InvokeAsync(lat);
        if (CenterLngChanged.HasDelegate)
            await CenterLngChanged.InvokeAsync(lng);
    }

    internal override bool HasAnyEventCallback =>
        HasBaseEventCallbacks || CenterLatChanged.HasDelegate || CenterLngChanged.HasDelegate;

    protected override string JsDisposeFunction => "blazorHerePlatform.objectManager.disposeCircleComponent";

    protected override Task RegisterWithMapAsync() => MapRef.AddCircle(this);

    protected override Task UnregisterFromMapAsync()
    {
        if (MapRef is not null)
            return MapRef.RemoveCircle(this);
        return Task.CompletedTask;
    }

    protected override async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateCircleComponent",
            [Guid,
            new CircleComponentOptions
            {
                CenterLat = CenterLat,
                CenterLng = CenterLng,
                Radius = Radius,
                StrokeColor = StrokeColor,
                FillColor = FillColor,
                LineWidth = LineWidth,
                Precision = Precision,
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
        return parameters.DidParameterChange(CenterLat) ||
            parameters.DidParameterChange(CenterLng) ||
            parameters.DidParameterChange(Radius) ||
            parameters.DidParameterChange(StrokeColor) ||
            parameters.DidParameterChange(FillColor) ||
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(Precision) ||
            parameters.DidParameterChange(LineCap) ||
            parameters.DidParameterChange(LineJoin) ||
            parameters.DidParameterChange(LineDash) ||
            parameters.DidParameterChange(LineDashOffset) ||
            parameters.DidParameterChange(ZIndex) ||
            parameters.DidParameterChange(Draggable) ||
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Visible);
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
