using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Events;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class MarkerComponent : MapObjectComponentBase
{
    internal string TemplateElementId => $"blz-mc-{Guid}";

    [Parameter, JsonIgnore]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Latitude in degrees. Two-way bindable via <c>@bind-Lat</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lat { get; set; }

    /// <summary>
    /// Callback for two-way binding of <see cref="Lat"/>.
    /// Fired when the marker is dragged to a new position.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<double> LatChanged { get; set; }

    /// <summary>
    /// Longitude in degrees. Two-way bindable via <c>@bind-Lng</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lng { get; set; }

    /// <summary>
    /// Callback for two-way binding of <see cref="Lng"/>.
    /// Fired when the marker is dragged to a new position.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<double> LngChanged { get; set; }

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
    /// URL or SVG string for a custom icon.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? IconUrl { get; set; }

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

    public async Task ForceRender()
    {
        if (!HasRendered) return;
        await UpdateOptions();
    }

    protected override string JsDisposeFunction => JsInteropIdentifiers.DisposeMarkerComponent;

    protected override Task RegisterWithMapAsync() => MapRef.AddMarker(this);

    protected override Task UnregisterFromMapAsync()
    {
        if (MapRef is not null)
            return MapRef.RemoveMarker(this);
        return Task.CompletedTask;
    }

    protected override async Task UpdateOptions()
    {
        var hasInfoBubbleContent = ChildContent is not null;
        await Js.InvokeAsync<string>(
            JsInteropIdentifiers.UpdateMarkerComponent,
            [Guid,
            new MarkerComponentOptions
            {
                Position = new LatLngLiteral(Lat, Lng),
                Draggable = Draggable,
                Clickable = Clickable || Draggable || HasAnyEventCallback || hasInfoBubbleContent,
                ZIndex = ZIndex,
                Opacity = Opacity,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                Visible = Visible,
                IconUrl = IconUrl,
                MapId = MapRef.MapId,
                InfoBubbleTemplateId = hasInfoBubbleContent ? TemplateElementId : null,
            },
            MapRef.CallbackRef]);
    }

    protected override bool CheckParameterChanges(ParameterView parameters)
    {
        return parameters.DidParameterChange(Lat) ||
            parameters.DidParameterChange(Lng) ||
            parameters.DidParameterChange(ZIndex) ||
            parameters.DidParameterChange(Opacity) ||
            parameters.DidParameterChange(MinZoom) ||
            parameters.DidParameterChange(MaxZoom) ||
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Draggable) ||
            parameters.DidParameterChange(Visible) ||
            parameters.DidParameterChange(IconUrl);
    }

    internal readonly struct MarkerComponentOptions
    {
        public LatLngLiteral? Position { get; init; }
        public Guid? MapId { get; init; }
        public bool Draggable { get; init; }
        public bool Clickable { get; init; }
        public int? ZIndex { get; init; }
        public double? Opacity { get; init; }
        public double? MinZoom { get; init; }
        public double? MaxZoom { get; init; }
        public bool Visible { get; init; }
        public string? IconUrl { get; init; }
        public string? InfoBubbleTemplateId { get; init; }
    }
}
