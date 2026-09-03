<p align="center">
  <img src="https://raw.githubusercontent.com/emuuu/HerePlatform.NET/main/here-platform.png" alt="HerePlatform.NET" width="128" />
</p>

<h1 align="center">HerePlatform.NET</h1>

.NET libraries for the [HERE Platform](https://developer.here.com/) APIs — targeting .NET 8, 9, and 10.

[![NuGet Blazor](https://img.shields.io/nuget/v/HerePlatform.NET.Blazor.svg?label=Blazor)](https://www.nuget.org/packages/HerePlatform.NET.Blazor)
[![NuGet RestClient](https://img.shields.io/nuget/v/HerePlatform.NET.RestClient.svg?label=RestClient)](https://www.nuget.org/packages/HerePlatform.NET.RestClient)
[![CI](https://github.com/emuuu/HerePlatform.NET/actions/workflows/ci.yml/badge.svg)](https://github.com/emuuu/HerePlatform.NET/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Docs](https://img.shields.io/badge/Docs-GitHub%20Pages-blue)](https://emuuu.github.io/HerePlatform.NET/)

> **Unofficial project.** HerePlatform.NET is an independent community library. It is not affiliated with, endorsed by, or supported by HERE Global B.V. See [Trademarks and disclaimer](#trademarks-and-disclaimer).

| Package | Description |
|---|---|
| **HerePlatform.NET.Blazor** | Blazor components for HERE Maps JS API (includes RestClient) |
| **HerePlatform.NET.RestClient** | Standalone REST client for HERE APIs — usable without Blazor |

## Prerequisites

- A [HERE Developer](https://developer.here.com/) account and API key
- .NET 8.0, 9.0, or 10.0

## Installation

```bash
# Blazor app (includes map components + all REST services)
dotnet add package HerePlatform.NET.Blazor

# Non-Blazor app (REST services only — console, API, Worker, etc.)
dotnet add package HerePlatform.NET.RestClient
```

## Quick Start — Blazor

Register services and add a map:

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

## Quick Start — REST Client

Use HERE APIs from any .NET app without Blazor:

```csharp
// Program.cs
builder.Services.AddHereRestServices(options =>
{
    options.ApiKey = "YOUR_HERE_API_KEY";
});
```

```csharp
// Inject and use any service
public class MyService(IRoutingService routing, IGeocodingService geocoding)
{
    public async Task Example()
    {
        var route = await routing.CalculateRouteAsync(new RoutingRequest
        {
            Origin = new LatLngLiteral(52.52, 13.405),
            Destination = new LatLngLiteral(48.8566, 2.3522),
            TransportMode = TransportMode.Car
        });

        var address = await geocoding.GeocodeAsync("Brandenburger Tor, Berlin");
    }
}
```

## REST Services

All services support dependency injection and are available in both the Blazor and standalone RestClient packages.

| Service | Methods | HERE API |
|---|---|---|
| `IRoutingService` | `CalculateRouteAsync` | Routing v8 |
| `IIsolineService` | `CalculateIsolineAsync` | Isoline Routing v8 |
| `IMatrixRoutingService` | `CalculateMatrixAsync` | Matrix Routing v8 |
| `IGeocodingService` | `GeocodeAsync`, `ReverseGeocodeAsync` | Geocoding & Search v7 |
| `IAutosuggestService` | `SuggestAsync`, `AutocompleteAsync` | Geocoding & Search v7 |
| `IPlacesService` | `DiscoverAsync`, `BrowseAsync`, `LookupAsync` | Geocoding & Search v7 |
| `ITrafficService` | `GetTrafficIncidentsAsync`, `GetTrafficFlowAsync` | Traffic v7 |
| `IPublicTransitService` | `GetDeparturesAsync`, `SearchStationsAsync` | Public Transit v8 |
| `IWaypointSequenceService` | `OptimizeSequenceAsync` | Waypoint Sequence v8 |
| `IGeofencingService` | `CheckPositionAsync` | Client-side |
| `IWeatherService` | `GetWeatherAsync` | Destination Weather v3 |
| `IRouteMatchingService` | `MatchRouteAsync` | Route Matching v8 |
| `IEvChargePointsService` | `SearchStationsAsync` | EV Charge Points v3 |
| `IMapImageService` | `GetImageAsync` | Map Image v3 |
| `IIntermodalRoutingService` | `CalculateRouteAsync` | Intermodal Routing v8 |
| `ITourPlanningService` | `SolveAsync` | Tour Planning v3 |

## Blazor Components

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

## Utilities

Available in both packages via `HerePlatform.Core`:

- **`WktParser`** — parse WKT geometry strings (Point, LineString, Polygon, MultiPoint)
- **`GeoJsonExporter`** — export coordinates to GeoJSON features and collections
- **`FlexiblePolyline`** — decode/encode HERE's flexible polyline format

## Documentation

Full documentation with interactive demos and API reference: **[emuuu.github.io/HerePlatform.NET](https://emuuu.github.io/HerePlatform.NET/)**

## Trademarks and disclaimer

HerePlatform.NET is an independent, community-maintained open-source project. It is not an official HERE product and is not affiliated with, authorized by, endorsed by, or sponsored by HERE Global B.V.

"HERE", "HERE Technologies", "HERE Maps" and the HERE logo are trademarks or registered trademarks of HERE Global B.V. All other product names, logos and brands are the property of their respective owners. They are used in this project, in its documentation and in its package names solely to identify the third-party APIs this library communicates with; such descriptive use implies no endorsement, sponsorship or business relationship.

The library is provided under the MIT license, as-is and without warranty of any kind. It carries no support agreement or service level of any kind from HERE, and using it does not change your obligations under HERE's own terms of service — you still need your own HERE account and API key. Questions about the APIs themselves belong to their respective providers; questions about this library belong in its [issue tracker](https://github.com/emuuu/HerePlatform.NET/issues).

If you hold rights to any of the marks named above and object to their use here, please open an issue and it will be addressed.

## License

MIT
