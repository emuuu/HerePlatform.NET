# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## HerePlatform.NET.Blazor

### [Unreleased]

### [1.1.0] - 2026-06-10

#### Fixed

- `HereAutosuggest` with default `AutosuggestOptions` always produced an HTTP 400 against the HERE API: the default `In = "countryCode:DEU"` was sent without the spatial context (`at`, `in=circle` or `in=bbox`) the Autosuggest API requires. `At` now defaults to the geographic center of Germany (51.1657, 10.4515)
- Invalid option combinations (`countryCode:` filter without `At`) are no longer sent and silently rejected — they are reported through the `OnError` callback with a descriptive message
- Non-authentication errors from the HERE Autosuggest JS callback (e.g. HTTP 400) are now forwarded to `OnError` instead of only being logged to the browser console
- `At` is omitted from requests when `In` contains a `circle:`/`bbox:` expression, since the HERE API treats `at` and `circle`/`bbox` as mutually exclusive

#### Added

- `AutosuggestOptions.EnsureValidForAutosuggest()` and `AutosuggestOptions.InProvidesSpatialContext()` to validate the At/In contract of the HERE Autosuggest API

#### Changed

- Updated ASP.NET Core dependencies to 10.0.9 / 9.0.17 / 8.0.28

### [1.0.1] - 2026-05-15

#### Fixed

- Hardened map dispose and JS callbacks against Blazor circuit disconnect

#### Changed

- Updated NuGet dependencies and added SourceLink support

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

### [1.1.0] - 2026-06-10

#### Fixed

- `IAutosuggestService.SuggestAsync` with default `AutosuggestOptions` always produced an HTTP 400: the default `In = "countryCode:DEU"` was sent without the spatial context (`at`, `in=circle` or `in=bbox`) the HERE Autosuggest API requires. `At` now defaults to the geographic center of Germany (51.1657, 10.4515)
- `SuggestAsync` throws a descriptive `InvalidOperationException` for a `countryCode:` filter without `At` instead of sending a request the API rejects
- `At` is omitted from requests when `In` contains a `circle:`/`bbox:` expression, since the HERE API treats `at` and `circle`/`bbox` as mutually exclusive
- Matrix Routing now matches the HERE v8 flat-array response format (`travelTimes`/`distances`/`errorCodes` instead of `entries[]`, `transportMode`+`routingMode` instead of `profile`, explicit `async=false`)
- Public Transit departures were missing the required radius suffix on the `in` parameter

#### Added

- `Taxi` transport mode and new routing avoid features: `DirtRoad`, `CarShuttleTrain`, `SeasonalClosure`, `DifficultTurns`
- `Alerts` and `NwsAlerts` weather products

#### Changed

- `AutocompleteAsync` is not validated (the Autocomplete API accepts a standalone `countryCode:` filter), but it now sends the default `At` as a ranking bias (`at` is a supported Autocomplete parameter). Set `At = null` to restore the previous behavior
- Updated Microsoft.Extensions dependencies to 10.0.9

### [1.0.0] - 2025-02-15

#### Added

- REST services: routing, isoline, matrix routing, geocoding, places, traffic, public transit, waypoint sequence, geofencing
- Utilities: `WktParser`, `GeoJsonExporter`, `FlexiblePolyline` codec
- Multi-target support for net8.0, net9.0, and net10.0

For older versions, see [GitHub Releases](https://github.com/emuuu/HerePlatform.NET/releases).

[1.0.0]: https://github.com/emuuu/HerePlatform.NET/releases/tag/v1.0.0
