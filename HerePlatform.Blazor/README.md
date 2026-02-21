![HerePlatform.NET](https://raw.githubusercontent.com/emuuu/HerePlatform.NET/main/here-platform.png)

# HerePlatform.NET.Blazor

Blazor components for the [HERE Maps JS API](https://developer.here.com/) — interactive maps, markers, shapes, routing, clustering, and more. Includes all REST services from the RestClient package. Targets .NET 8, 9, and 10.

[![NuGet](https://img.shields.io/nuget/v/HerePlatform.NET.Blazor.svg)](https://www.nuget.org/packages/HerePlatform.NET.Blazor)

## Installation

```bash
dotnet add package HerePlatform.NET.Blazor
```

## Usage

```csharp
// Program.cs
builder.Services.AddHerePlatformBlazor("YOUR_HERE_API_KEY");
```

```razor
<AdvancedHereMap Id="myMap" Height="400px" Options="@(new MapOptions { Zoom = 12 })">
    <MarkerComponent Lat="52.52" Lng="13.405">
        <b>Hello Berlin!</b>
    </MarkerComponent>
</AdvancedHereMap>
```

All required JavaScript is loaded automatically — no manual `<script>` tag needed. Blazor Server and WebAssembly are both supported.

## Components

| Component | Description |
|---|---|
| `HereMap` / `AdvancedHereMap` | Map with two-way binding (center, zoom, tilt, heading) and rich events |
| `MarkerComponent` / `DomMarkerComponent` | Standard and HTML markers with drag, click, InfoBubbles |
| `PolylineComponent`, `PolygonComponent`, `CircleComponent`, `RectComponent` | Shapes — all draggable and styleable |
| `RouteComponent` | Declarative route rendering on the map |
| `MarkerClusterComponent` | Marker clustering with custom SVG templates |
| `HeatmapComponent` | Heatmap visualization |
| `InfoBubbleComponent` | Standalone InfoBubbles |
| `GroupComponent` | Group map objects, toggle visibility |
| `GeoJsonReaderComponent` / `KmlReaderComponent` | Load GeoJSON/KML data |
| `CustomTileLayerComponent` / `ImageOverlayComponent` | Custom tile layers and image overlays |
| `HereAutosuggest` | Address search with keyboard navigation and customizable design |
| `DistanceMeasurementComponent`, `OverviewMapComponent`, `ZoomRectangleComponent` | UI controls |
| `ContextMenuComponent` | Right-click context menu |

## Included REST Services

This package includes all REST services from `HerePlatform.NET.RestClient` — routing, geocoding, places, traffic, transit, and more.

## Links

- [Full documentation](https://emuuu.github.io/HerePlatform.NET/)
- [GitHub repository](https://github.com/emuuu/HerePlatform.NET)
