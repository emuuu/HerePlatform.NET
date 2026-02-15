<p align="center">
  <img src="blazor-here-platform.svg" alt="BlazorHerePlatform" width="128" />
</p>

# BlazorHerePlatform

A comprehensive Blazor component library for the [HERE Maps JS API](https://developer.here.com/develop/javascript-api). Declarative map rendering, markers, shapes, clustering, heatmaps, routing, geocoding, and more — all as Blazor components and services.

[![NuGet](https://img.shields.io/nuget/v/BlazorHerePlatform.svg)](https://www.nuget.org/packages/BlazorHerePlatform)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorHerePlatform.svg)](https://www.nuget.org/packages/BlazorHerePlatform)
[![CI](https://github.com/emuuu/BlazorHerePlatform/actions/workflows/ci.yml/badge.svg)](https://github.com/emuuu/BlazorHerePlatform/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Feature highlights:**

- Interactive map components with two-way binding (center, zoom, tilt, heading)
- Markers (standard and DOM-based) with drag, click events, and InfoBubbles
- Shapes: polylines, polygons, circles, rectangles — all draggable and styleable
- Routing with car, truck, pedestrian, bicycle, scooter, and EV support
- Geocoding and reverse geocoding
- Traffic incidents and flow data
- Marker clustering with custom SVG templates
- Heatmap visualization
- Autosuggest search component with customizable templates
- GeoJSON, KML, custom tile layers, and image overlays
- Isoline (isochrone), matrix routing, waypoint optimization, geofencing, places, and public transit services
- Utility classes: WKT parser, GeoJSON exporter, flexible polyline codec
- Blazor Server and Blazor WebAssembly support

## Prerequisites

- A [HERE Developer](https://developer.here.com/) account and API key
- .NET 8.0, 9.0, or 10.0

## Installation

```bash
dotnet add package BlazorHerePlatform
```

## Getting Started

### 1. Register services

In `Program.cs`, register BlazorHerePlatform with your API key:

```csharp
builder.Services.AddBlazorHerePlatform("YOUR_HERE_API_KEY");
```

For advanced configuration, use `HereApiLoadOptions`:

```csharp
builder.Services.AddBlazorHerePlatform(new HereApiLoadOptions("YOUR_HERE_API_KEY")
{
    LoadClustering = true,  // Enable marker clustering
    LoadData = true,        // Enable GeoJSON/KML support
    Language = "en"
});
```

### 2. Add a map

Place a map component in any Razor page:

```razor
<AdvancedHereMap Id="myMap" Height="400px" Options="@(new MapOptions { Zoom = 12 })">
    <MarkerComponent Lat="52.52" Lng="13.405">
        <b>Hello Berlin!</b>
    </MarkerComponent>
</AdvancedHereMap>
```

### 3. Blazor Server: increase SignalR message size

Blazor Server apps must increase the SignalR maximum receive message size to handle map data:

```csharp
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 1024 * 1024);
```

## Map Components

### HereMap

Minimal map component. Provides programmatic access via `InteropObject` after initialization.

```razor
<HereMap Id="basic" Height="400px" Options="@_options" OnAfterInit="OnMapReady" />

@code {
    private MapOptions _options = new() { Zoom = 10 };

    private void OnMapReady()
    {
        // Map is ready for programmatic interaction
    }
}
```

Use `ReInitializeAsync()` to rebuild the map when options like `UiLocale` change at runtime.

### AdvancedHereMap

Full-featured declarative wrapper around `HereMap`. Supports child components, two-way binding, and a rich event model.

```razor
<AdvancedHereMap Id="advanced"
                 Height="500px"
                 Options="@_options"
                 @bind-Center="_center"
                 @bind-Zoom="_zoom"
                 Animate="true"
                 OnClick="HandleMapClick">
    @* Child components: markers, shapes, clusters, etc. *@
</AdvancedHereMap>

@code {
    private MapOptions _options = new() { LayerType = MapLayerType.VectorNormalMap };
    private LatLngLiteral _center = new(52.52, 13.405);
    private double _zoom = 12;

    private void HandleMapClick(MapPointerEventArgs e)
    {
        if (e.Position is { } pos)
            _center = pos;
    }
}
```

**Two-way bindable:** `@bind-Center`, `@bind-Zoom`, `@bind-Tilt`, `@bind-Heading`

**Key parameters:**

| Parameter | Type | Description |
|---|---|---|
| `Animate` | `bool` | Animate programmatic view changes |
| `DisabledBehaviors` | `BehaviorFeature` | Disable interactions (Panning, WheelZoom, Tilt, etc.) |
| `ViewportPadding` | `Padding` | Viewport padding applied after init |
| `OnClick` | `EventCallback<MapPointerEventArgs>` | Map background clicked |

The component also exposes `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, drag events, view change events, and pointer events. See IntelliSense for the full list.

### MapOptions

| Property | Type | Default | Description |
|---|---|---|---|
| `Center` | `LatLngLiteral?` | `null` | Initial map center |
| `Zoom` | `double` | `10` | Initial zoom level |
| `LayerType` | `MapLayerType` | `VectorNormalMap` | Base layer type |
| `EnableInteraction` | `bool` | `true` | Enable user interaction |
| `EnableUI` | `bool` | `true` | Show built-in UI controls |
| `UiLocale` | `string?` | `null` | UI locale (e.g. `"de-DE"`, `"fr-FR"`) |
| `Tilt` | `double?` | `null` | Tilt angle in degrees |
| `Heading` | `double?` | `null` | Map rotation in degrees |
| `MinZoom` / `MaxZoom` | `double?` | `null` | Zoom constraints |
| `PixelRatio` | `double?` | `null` | High-DPI pixel ratio |

### Switching Layers

Change the base layer at runtime:

```csharp
// Switch to satellite
advancedMap.MapRef.Options.LayerType = MapLayerType.RasterSatelliteMap;
await advancedMap.MapRef.ReInitializeAsync();
```

Add or remove overlay layers (e.g. traffic):

```csharp
await advancedMap.AddLayerAsync("vector.normal.traffic");
await advancedMap.RemoveLayerAsync("vector.normal.traffic");
```

## Markers

### MarkerComponent

Standard HERE marker with optional InfoBubble via `ChildContent`.

```razor
<AdvancedHereMap Id="markerDemo" Height="400px" Options="@(new MapOptions())">
    <MarkerComponent @bind-Lat="_lat" @bind-Lng="_lng"
                     Draggable="true"
                     Clickable="true"
                     OnTap="HandleTap"
                     IconUrl="/images/custom-pin.svg">
        <div style="padding: 8px;">
            <strong>Draggable marker</strong><br/>
            Position: @_lat.ToString("F4"), @_lng.ToString("F4")
        </div>
    </MarkerComponent>
</AdvancedHereMap>

@code {
    private double _lat = 52.52;
    private double _lng = 13.405;

    private void HandleTap(MapPointerEventArgs e) { /* ... */ }
}
```

**Key parameters:** `Lat`/`Lng` (two-way), `Draggable`, `Clickable`, `Opacity`, `MinZoom`/`MaxZoom`, `IconUrl`, `Visible`, `Title`, `ZIndex`

### DomMarkerComponent

Custom HTML marker rendered as a DOM element. The `ChildContent` defines the static HTML template for the marker icon.

```razor
<DomMarkerComponent Lat="52.52" Lng="13.405">
    <div style="background: #0066FF; color: white; padding: 4px 8px; border-radius: 4px;">
        Custom HTML
    </div>
</DomMarkerComponent>
```

> **Note:** ChildContent is rendered as static HTML. Blazor interactivity (event handlers, binding) inside DomMarker content is not supported.

## Shapes

All shape components support `Draggable`, `Clickable`, `Visible`, `ZIndex`, and `Data`. Drag updates are reflected via two-way binding.

### PolylineComponent

```razor
<PolylineComponent Path="@_path"
                   StrokeColor="#0066FF"
                   LineWidth="4"
                   Arrows="true"
                   LineDash="@(new double[] { 10, 5 })" />
```

**Key parameters:** `Path` (two-way), `StrokeColor`, `LineWidth`, `LineCap`, `LineDash`, `Arrows`, `Extrusion`/`Elevation` (3D with HARP engine)

### PolygonComponent

```razor
<PolygonComponent Path="@_polygon"
                  FillColor="rgba(0, 102, 255, 0.3)"
                  StrokeColor="#0066FF"
                  LineWidth="2" />
```

Supports `Holes` for interior cutouts and `Extrusion`/`Elevation` for 3D buildings.

### CircleComponent

```razor
<CircleComponent @bind-CenterLat="_lat"
                 @bind-CenterLng="_lng"
                 Radius="500"
                 FillColor="rgba(255, 0, 0, 0.2)"
                 StrokeColor="red"
                 Draggable="true" />
```

### RectComponent

```razor
<RectComponent @bind-Top="_top" @bind-Left="_left"
               @bind-Bottom="_bottom" @bind-Right="_right"
               FillColor="rgba(0, 255, 0, 0.2)"
               Draggable="true" />
```

## InfoBubbles

### Declarative (via MarkerComponent)

Place HTML inside a `MarkerComponent`'s `ChildContent` — the InfoBubble opens on marker tap:

```razor
<MarkerComponent Lat="52.52" Lng="13.405" Clickable="true">
    <div>This appears in an InfoBubble on click.</div>
</MarkerComponent>
```

### Standalone InfoBubbleComponent

For InfoBubbles independent of markers:

```razor
<InfoBubbleComponent Lat="52.52" Lng="13.405" @bind-IsOpen="_isOpen" AutoPan="true">
    <div style="padding: 8px;">Standalone InfoBubble</div>
</InfoBubbleComponent>

@code {
    private bool _isOpen = true;
}
```

> **Note:** ChildContent is rendered as static HTML.

## Groups

`GroupComponent` groups map objects together. Toggle `Visible` to show/hide all children at once.

```razor
<GroupComponent Visible="@_showPois">
    <MarkerComponent Lat="52.52" Lng="13.405" />
    <MarkerComponent Lat="52.53" Lng="13.41" />
    <CircleComponent CenterLat="52.52" CenterLng="13.405" Radius="200" />
</GroupComponent>

@code {
    private bool _showPois = true;
}
```

Use `GetBoundsAsync()` to get the bounding rectangle of all objects in the group.

## Clustering

Requires `LoadClustering = true` in `HereApiLoadOptions`.

```razor
<AdvancedHereMap Id="clusterMap" Height="400px" Options="@(new MapOptions())">
    <MarkerClusterComponent DataPoints="@_points"
                            Eps="32"
                            MinWeight="2"
                            OnClusterTap="HandleClusterTap"
                            OnNoiseTap="HandleNoiseTap" />
</AdvancedHereMap>

@code {
    private List<ClusterDataPoint> _points = new()
    {
        new(52.52, 13.405),
        new(52.521, 13.406),
        new(52.53, 13.41, weight: 3),
    };

    private void HandleClusterTap(ClusterTapEventArgs e) { /* Zoom into cluster */ }
    private void HandleNoiseTap(ClusterTapEventArgs e) { /* Show detail */ }
}
```

Customize cluster visuals with `ClusterSvgTemplate` and `NoiseSvgTemplate` using `{count}` and `{color}` placeholders.

## Data Visualization

### HeatmapComponent

```razor
<HeatmapComponent DataPoints="@_heatData"
                  Opacity="0.6"
                  Colors="@(new Dictionary<double, string>
                  {
                      { 0.0, "blue" },
                      { 0.5, "yellow" },
                      { 1.0, "red" }
                  })" />

@code {
    private List<HeatmapDataPoint> _heatData = new()
    {
        new(52.52, 13.405, 0.8),
        new(52.53, 13.41, 1.0),
    };
}
```

### GeoJsonReaderComponent

Requires `LoadData = true` in `HereApiLoadOptions`.

```razor
<GeoJsonReaderComponent Url="/data/districts.geojson"
                        DefaultStyle="@(new StyleOptions { FillColor = "rgba(0,102,255,0.3)", StrokeColor = "#0066FF" })"
                        OnLoaded="e => Console.WriteLine($&quot;Loaded {e.ObjectCount} objects&quot;)" />
```

Alternatively, pass inline GeoJSON via the `GeoJsonString` parameter.

### KmlReaderComponent

Requires `LoadData = true`.

```razor
<KmlReaderComponent Url="/data/route.kml" />
```

### CustomTileLayerComponent

Overlay third-party XYZ tile layers:

```razor
<CustomTileLayerComponent UrlPattern="https://tile.openstreetmap.org/{z}/{x}/{y}.png"
                          Opacity="0.5"
                          Min="0"
                          Max="18" />
```

### ImageOverlayComponent

Pin an image to geographic bounds:

```razor
<ImageOverlayComponent ImageUrl="/images/overlay.png"
                       Top="52.55" Left="13.35"
                       Bottom="52.50" Right="13.45"
                       Opacity="0.7" />
```

## HERE REST Services

All services are registered automatically via `AddBlazorHerePlatform()` and can be injected with standard DI.

| Service | Methods | Description |
|---|---|---|
| `IRoutingService` | `CalculateRouteAsync` | Route calculation (car, truck, pedestrian, bicycle, scooter, EV) |
| `IIsolineService` | `CalculateIsolineAsync` | Isochrone/isodistance polygons |
| `IMatrixRoutingService` | `CalculateMatrixAsync` | Many-to-many distance/duration matrix |
| `IGeocodingService` | `GeocodeAsync`, `ReverseGeocodeAsync` | Address lookup and reverse geocoding |
| `IPlacesService` | `DiscoverAsync`, `BrowseAsync`, `LookupAsync` | Place search, category browse, ID lookup |
| `ITrafficService` | `GetTrafficIncidentsAsync`, `GetTrafficFlowAsync` | Real-time traffic data |
| `IPublicTransitService` | `GetDeparturesAsync`, `SearchStationsAsync` | Public transit departures and station search |
| `IWaypointSequenceService` | `OptimizeSequenceAsync` | Optimal waypoint ordering (TSP) |
| `IGeofencingService` | `CheckPositionAsync` | Check position against geofence zones |

### Routing Example

**Programmatic:**

```razor
@inject IRoutingService RoutingService

@code {
    private async Task CalculateRoute()
    {
        var result = await RoutingService.CalculateRouteAsync(new RoutingRequest
        {
            Origin = new LatLngLiteral(52.52, 13.405),
            Destination = new LatLngLiteral(48.8566, 2.3522),
            TransportMode = TransportMode.Car,
            RoutingMode = RoutingMode.Fast,
            Avoid = RoutingAvoidFeature.Tolls | RoutingAvoidFeature.Ferries,
            ReturnInstructions = true
        });

        var route = result.Routes?.FirstOrDefault();
        var summary = route?.Sections?.FirstOrDefault()?.Summary;
        // summary.Duration (seconds), summary.Length (meters)
    }
}
```

**Declarative (RouteComponent):**

```razor
<AdvancedHereMap Id="routeMap" Height="400px" Options="@(new MapOptions())">
    <RouteComponent Origin="@(new LatLngLiteral(52.52, 13.405))"
                    Destination="@(new LatLngLiteral(48.8566, 2.3522))"
                    TransportMode="TransportMode.Car"
                    StrokeColor="#0066FF"
                    LineWidth="5"
                    OnRouteCalculated="HandleResult" />
</AdvancedHereMap>
```

Truck routing supports `TruckOptions` (height, width, length, gross weight, hazardous goods) and EV routing supports `EvOptions` (battery charge, consumption tables, charging curves).

## Autosuggest

Address search component with keyboard navigation and customizable design.

```razor
<HereAutosuggest Placeholder="Search address..."
                 Options="@(new AutosuggestOptions { Limit = 5, Lang = "en", In = "countryCode:DEU" })"
                 OnItemSelected="HandleSelection"
                 Design="AutosuggestDesign.Outlined"
                 DebounceMs="300" />

@code {
    private void HandleSelection(AutosuggestItem item)
    {
        // item.Title, item.Position, item.Address
    }
}
```

**Design variants:** `Default`, `Compact`, `Filled`, `Outlined`, `Rounded`

Customize rendering with `SuggestionItemTemplate`, `SuggestionListTemplate`, and `InputTemplate` render fragments. CSS custom properties from `here-autosuggest.css` can be overridden for further styling.

## UI Controls

Place these inside `AdvancedHereMap` to add interactive tools.

### DistanceMeasurementComponent

```razor
<DistanceMeasurementComponent Active="true"
                              Alignment="UIAlignment.TopRight"
                              OnMeasured="d => _distance = d" />
```

### OverviewMapComponent

```razor
<OverviewMapComponent Alignment="UIAlignment.BottomRight" ZoomDelta="4" />
```

### ZoomRectangleComponent

```razor
<ZoomRectangleComponent Active="true" Alignment="UIAlignment.TopRight" />
```

**UIAlignment options:** `TopLeft`, `TopCenter`, `TopRight`, `LeftTop`, `LeftMiddle`, `LeftBottom`, `RightTop`, `RightMiddle`, `RightBottom`, `BottomLeft`, `BottomCenter`, `BottomRight`

## Context Menu

Add a right-click context menu to the map:

```razor
<AdvancedHereMap Id="ctxMap" Height="400px" Options="@(new MapOptions())">
    <ContextMenuComponent OnItemClick="HandleMenuItem">
        <ContextMenuItem Label="Add Marker" />
        <ContextMenuItem Label="Measure Distance" />
        <ContextMenuItem Label="Disabled Item" Disabled="true" />
    </ContextMenuComponent>
</AdvancedHereMap>

@code {
    private void HandleMenuItem(ContextMenuEventArgs e)
    {
        // e.ItemLabel, e.Position, e.ViewportX, e.ViewportY
    }
}
```

## Utilities

### WktParser

Parse Well-Known Text (WKT) geometry strings:

```csharp
LatLngLiteral? point = WktParser.ParsePoint("POINT(13.405 52.52)");
List<LatLngLiteral> line = WktParser.ParseLineString("LINESTRING(13.4 52.5, 13.5 52.6)");
List<List<LatLngLiteral>> polygon = WktParser.ParsePolygon("POLYGON((13.4 52.5, 13.5 52.5, 13.5 52.6, 13.4 52.5))");
List<LatLngLiteral> multi = WktParser.ParseMultiPoint("MULTIPOINT(13.4 52.5, 13.5 52.6)");
```

### GeoJsonExporter

Export coordinates to GeoJSON:

```csharp
string pointJson = GeoJsonExporter.ToGeoJsonFeature(new LatLngLiteral(52.52, 13.405));
string lineJson = GeoJsonExporter.ToLineStringFeature(coordinates);
string polygonJson = GeoJsonExporter.ToPolygonFeature(ring, holes);
string collection = GeoJsonExporter.ToFeatureCollection(new[] { pointJson, lineJson });
```

### FlexiblePolyline

Decode and encode HERE's flexible polyline format (used in routing responses):

```csharp
List<LatLngLiteral> decoded = FlexiblePolyline.Decode(encodedString);
string encoded = FlexiblePolyline.Encode(coordinates, precision: 5);
```

## Advanced Configuration

### HereApiLoadOptions

| Property | Type | Default | Description |
|---|---|---|---|
| `ApiKey` | `string` | *(required)* | HERE API key |
| `Version` | `string` | `"3.1"` | HERE Maps JS API version |
| `LoadMapEvents` | `bool` | `true` | Load map events module |
| `LoadUI` | `bool` | `true` | Load UI module |
| `LoadClustering` | `bool` | `false` | Load clustering module |
| `LoadData` | `bool` | `false` | Load data module (GeoJSON/KML) |
| `UseHarpEngine` | `bool` | `true` | Use HARP rendering engine (WebGL) |
| `Language` | `string?` | `null` | Default language |

### Single API key per page

The HERE Maps JS API is loaded once per page with a single API key. Multiple keys on the same page are not supported.

## Repository Setup

After forking or cloning, configure the following GitHub settings:

### Branch Protection Rules (main)

- Require at least 1 approval before merging
- Require the **CI** status check to pass
- Require branches to be up to date before merging
- Do not allow force pushes

### GitHub Pages

- Go to **Settings > Pages**
- Set **Source** to **GitHub Actions**

### Secrets

- `NUGET_API_KEY` — required for the release workflow to push packages to NuGet.org

## License

MIT
