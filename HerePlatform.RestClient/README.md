![HerePlatform.NET](https://raw.githubusercontent.com/emuuu/HerePlatform.NET/main/here-platform.png)

# HerePlatform.NET.RestClient

Standalone .NET REST client for the [HERE Platform](https://developer.here.com/) APIs — usable from any .NET app (console, API, Worker, etc.) without Blazor. Targets .NET 8, 9, and 10.

[![NuGet](https://img.shields.io/nuget/v/HerePlatform.NET.RestClient.svg)](https://www.nuget.org/packages/HerePlatform.NET.RestClient)

## Installation

```bash
dotnet add package HerePlatform.NET.RestClient
```

## Usage

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

## Available Services

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

## Links

- [Full documentation](https://emuuu.github.io/HerePlatform.NET/)
- [GitHub repository](https://github.com/emuuu/HerePlatform.NET)
