# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## HerePlatform.NET.Blazor

### [Unreleased]

### [1.0.0] - 2025-02-15

#### Added

- Interactive map components (`HereMap`, `AdvancedHereMap`) with two-way binding for center, zoom, tilt, and heading
- Marker components (`MarkerComponent`, `DomMarkerComponent`) with drag, click events, and InfoBubbles
- Shape components: `PolylineComponent`, `PolygonComponent`, `CircleComponent`, `RectComponent`
- `InfoBubbleComponent` for standalone InfoBubbles
- `GroupComponent` for grouping map objects
- `MarkerClusterComponent` with customizable SVG templates
- `HeatmapComponent` for data visualization
- `GeoJsonReaderComponent` and `KmlReaderComponent` for data layers
- `CustomTileLayerComponent` and `ImageOverlayComponent`
- `RouteComponent` for declarative routing
- `HereAutosuggest` search component with multiple design variants
- UI controls: `DistanceMeasurementComponent`, `OverviewMapComponent`, `ZoomRectangleComponent`
- `ContextMenuComponent` for right-click context menus
- Blazor Server and Blazor WebAssembly support

## HerePlatform.NET.RestClient

### [Unreleased]

### [1.0.0] - 2025-02-15

#### Added

- REST services: routing, isoline, matrix routing, geocoding, places, traffic, public transit, waypoint sequence, geofencing
- Utilities: `WktParser`, `GeoJsonExporter`, `FlexiblePolyline` codec
- Multi-target support for net8.0, net9.0, and net10.0

For older versions, see [GitHub Releases](https://github.com/emuuu/HerePlatform.NET/releases).

[1.0.0]: https://github.com/emuuu/HerePlatform.NET/releases/tag/v1.0.0
