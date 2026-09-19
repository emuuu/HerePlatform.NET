# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## HerePlatform.NET.Blazor

### [Unreleased]

### [1.3.1] - 2026-09-20

#### Fixed

- `HereAutosuggest`: 1.3.0 suppressed EVERY keystroke while the suggestion dropdown was open — in the
  default input template, and in any custom `InputTemplate` that followed the 1.3.0 guidance. Typing,
  Backspace/Delete, Tab and Home/End stopped working as soon as the first suggestions appeared, so the
  input could not be edited any further. Blazor's `@onkeydown:preventDefault` is a static per-event and
  per-element flag and cannot be limited to single keys. The browser default is now suppressed
  key-selectively by a `keydown` listener on the input, and only for `Enter` (implicit form submit) and
  `ArrowUp`/`ArrowDown` (caret jump), and only while the dropdown is open. Enter semantics are unchanged:
  it still picks the active suggestion, or the first one when none is active

#### Deprecated

- `AutosuggestInputContext.PreventDefaultKeyDown`: always returns `false` now and must no longer be applied
  via `@onkeydown:preventDefault` in a custom `InputTemplate` — the component handles the keyboard defaults
  itself. A custom template only has to splat `@attributes="context.InputAttributes"`, which carries the new
  `data-here-autosuggest-input` marker the component's keydown listener needs. The property is scheduled for
  removal in the next major version

### [1.3.0] - 2026-09-19

> **Known broken — do not use.** With the default input template — and with every custom `InputTemplate`
> that followed the 1.3.0 guidance to apply `@onkeydown:preventDefault` — an open suggestion dropdown
> suppressed every keystroke, leaving the input uneditable after the first suggestions appeared.
> Fixed in the following release.

#### Added

- `AutosuggestInputContext.PreventDefaultKeyDown`: exposes whether the current `onkeydown` must suppress the
  browser default action (`true` whenever the suggestion dropdown is open), so a custom `InputTemplate` can
  apply `@onkeydown:preventDefault="@context.PreventDefaultKeyDown"` — `@attributes` splatting alone cannot
  express that event modifier

#### Changed

- `HereAutosuggest`: pressing Enter while the suggestion dropdown is open now always selects a suggestion —
  the active one if navigated to with arrow keys, otherwise the first result. Previously, Enter without an
  active item did nothing and let the keystroke reach the surrounding `<form>`/`EditForm`

#### Fixed

- `HereAutosuggest` with a custom `InputTemplate`: pressing Enter while the dropdown was open selected a
  suggestion AND submitted a surrounding `<form>`/`EditForm`, because `InputAttributes` splatting could not
  carry the default template's `@onkeydown:preventDefault` modifier. Consumers must now also bind
  `@onkeydown:preventDefault="@context.PreventDefaultKeyDown"` on their custom input

### [1.2.0] - 2026-06-11

#### Added

- `AutosuggestOptions.Show` (default `"details"`): forwards the HERE `show` parameter. Without `show=details` the Autosuggest API returns only `address.label` — all structured fields on `AutosuggestAddress` (street, postal code, city, …) stayed null. Set `Show = null` to restore the previous label-only behavior
- `AutosuggestAddress.StateCode`, `County` and `CountyCode`, returned by the API with `show=details`

#### Changed

- Default Autosuggest requests now include `show=details`, so responses carry the full structured address. Consumers that assert exact request URLs or expect empty structured address fields must opt out via `Show = null`

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

### [1.2.0] - 2026-06-11

#### Added

- `AutosuggestOptions.Show` (default `"details"`): forwards the HERE `show` parameter on Autosuggest requests. Without `show=details` the Autosuggest API returns only `address.label` — all structured fields on `AutosuggestAddress` (street, postal code, city, …) stayed null. Set `Show = null` to restore the previous label-only behavior. `AutocompleteAsync` never sends the parameter (the Autocomplete API does not support `show=details` and returns structured addresses natively)
- `AutosuggestAddress.StateCode`, `County` and `CountyCode`, returned by the API with `show=details`

#### Changed

- Default Autosuggest requests now include `show=details`, so responses carry the full structured address. Consumers that assert exact request URLs or expect empty structured address fields must opt out via `Show = null`

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
