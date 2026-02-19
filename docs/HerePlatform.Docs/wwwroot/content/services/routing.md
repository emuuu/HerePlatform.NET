---
title: Routing Service
category: Services
order: 1
description: "Calculate routes between waypoints."
apiRef: IRoutingService
demo: map-routing
---

## Setup

Inject `IRoutingService` into your component. It is registered automatically by `AddBlazorHerePlatform`.

```csharp
@inject IRoutingService RoutingService
```

## CalculateRouteAsync

Calculate a route between an origin and destination. The service returns decoded polyline coordinates and section summaries.

```csharp
var request = new RoutingRequest
{
    Origin = new LatLngLiteral(52.5200, 13.4050),
    Destination = new LatLngLiteral(48.8566, 2.3522),
    TransportMode = TransportMode.Car
};

var result = await RoutingService.CalculateRouteAsync(request);

foreach (var route in result.Routes ?? [])
{
    foreach (var section in route.Sections ?? [])
    {
        var duration = section.Summary?.Duration; // seconds
        var length = section.Summary?.Length;       // meters
        var polyline = section.DecodedPolyline;     // List<LatLngLiteral>
    }
}
```

## RoutingRequest Options

`RoutingRequest` supports the following properties:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Origin` | `LatLngLiteral` | -- | Start position (required). |
| `Destination` | `LatLngLiteral` | -- | End position (required). |
| `Via` | `List<LatLngLiteral>?` | `null` | Intermediate waypoints. |
| `TransportMode` | `TransportMode` | `Car` | `Car`, `Truck`, `Pedestrian`, `Bicycle`, `Scooter`. |
| `RoutingMode` | `RoutingMode` | `Fast` | `Fast` (time) or `Short` (distance). |
| `Alternatives` | `int` | `0` | Number of alternative routes. |
| `ReturnPolyline` | `bool` | `true` | Include decoded polyline coordinates. |
| `ReturnInstructions` | `bool` | `false` | Include turn-by-turn instructions. |
| `Avoid` | `RoutingAvoidFeature` | `None` | Flags: `Tolls`, `Highways`, `Ferries`, `Tunnels`. |
| `Truck` | `TruckOptions?` | `null` | Truck-specific parameters (Truck mode only). |
| `Ev` | `EvOptions?` | `null` | Electric vehicle parameters. |

```csharp
var request = new RoutingRequest
{
    Origin = new LatLngLiteral(52.52, 13.405),
    Destination = new LatLngLiteral(48.8566, 2.3522),
    TransportMode = TransportMode.Car,
    RoutingMode = RoutingMode.Fast,
    Alternatives = 2,
    Avoid = RoutingAvoidFeature.Tolls | RoutingAvoidFeature.Ferries,
    ReturnInstructions = true
};
```

## Route Result

`RoutingResult` contains a list of `Route` objects, each with one or more `RouteSection` entries:

- **Polyline** -- encoded flexible polyline string.
- **DecodedPolyline** -- `List<LatLngLiteral>` coordinates for rendering.
- **Summary** -- `RouteSummary` with `Duration` (seconds), `Length` (meters), and optional `BaseDuration` (without traffic).
- **TurnByTurnActions** -- `List<TurnInstruction>` when `ReturnInstructions` is enabled.

```csharp
var route = result.Routes?.FirstOrDefault();
var section = route?.Sections?.FirstOrDefault();

if (section?.Summary is { } summary)
{
    Console.WriteLine($"Duration: {summary.Duration / 60} min");
    Console.WriteLine($"Distance: {summary.Length / 1000.0:F1} km");
}
```
