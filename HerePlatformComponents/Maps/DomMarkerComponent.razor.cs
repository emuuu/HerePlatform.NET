using HerePlatformComponents.Maps.Events;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// DomMarker component that renders a marker using HTML (DomIcon).
/// ChildContent provides the HTML template for the DomIcon.
/// Note: ChildContent is read as static HTML — no live Blazor interactivity inside the DomIcon.
/// </summary>
public partial class DomMarkerComponent : MapObjectComponentBase
{
    internal string TemplateElementId => $"blz-dm-{Guid}";

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

    internal override async Task HandleDragEvent(string eventName, MapDragEventArgs args)
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

        await base.HandleDragEvent(eventName, args);
    }

    internal override bool HasAnyEventCallback =>
        HasBaseEventCallbacks || LatChanged.HasDelegate || LngChanged.HasDelegate;

    protected override string JsDisposeFunction => "blazorHerePlatform.objectManager.disposeDomMarkerComponent";

    protected override Task RegisterWithMapAsync() => MapRef.AddDomMarker(this);

    protected override Task UnregisterFromMapAsync()
    {
        if (MapRef is not null)
            return MapRef.RemoveDomMarker(this);
        return Task.CompletedTask;
    }

    protected override async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateDomMarkerComponent",
            [Guid,
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
            MapRef.CallbackRef]);
    }

    protected override bool CheckParameterChanges(ParameterView parameters)
    {
        return parameters.DidParameterChange(Lat) ||
            parameters.DidParameterChange(Lng) ||
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Draggable) ||
            parameters.DidParameterChange(ZIndex) ||
            parameters.DidParameterChange(Opacity) ||
            parameters.DidParameterChange(MinZoom) ||
            parameters.DidParameterChange(MaxZoom) ||
            parameters.DidParameterChange(Visible);
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
