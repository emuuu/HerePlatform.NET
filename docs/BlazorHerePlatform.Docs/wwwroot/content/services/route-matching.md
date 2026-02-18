---
title: Route Matching Service
category: Services
order: 12
description: "Match GPS traces to road segments."
apiRef: IRouteMatchingService
---

## Setup

Inject `IRouteMatchingService` into your component. It is registered automatically by `AddBlazorHerePlatform`.

```csharp
@inject IRouteMatchingService RouteMatchingService
```

## MatchRouteAsync

Match a sequence of GPS trace points to road links on the HERE map. Useful for fleet telemetry, trip reconstruction, and speed analysis.

```csharp
var result = await RouteMatchingService.MatchRouteAsync(new RouteMatchingRequest
{
    TracePoints =
    [
        new TracePoint
        {
            Position = new LatLngLiteral(52.5308, 13.3847),
            Timestamp = DateTimeOffset.Parse("2025-01-15T08:00:00Z")
        },
        new TracePoint
        {
            Position = new LatLngLiteral(52.5280, 13.3900),
            Timestamp = DateTimeOffset.Parse("2025-01-15T08:01:00Z"),
            Speed = 30
        },
        new TracePoint
        {
            Position = new LatLngLiteral(52.5250, 13.3950),
            Timestamp = DateTimeOffset.Parse("2025-01-15T08:02:00Z")
        }
    ],
    TransportMode = TransportMode.Car
});

foreach (var link in result.MatchedLinks ?? [])
{
    Console.WriteLine($"Link {link.LinkId}: confidence {link.Confidence:P0}");
    Console.WriteLine($"  Speed limit: {link.SpeedLimit} km/h, FC {link.FunctionalClass}");
}
```

## RouteMatchingRequest Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TracePoints` | `List<TracePoint>?` | `null` | Ordered GPS points (required). |
| `TransportMode` | `TransportMode` | `Car` | `Car`, `Truck`, `Pedestrian`, `Bicycle`, `Scooter`. |

## TracePoint

Each trace point represents a GPS fix:

| Property | Type | Description |
|----------|------|-------------|
| `Position` | `LatLngLiteral` | GPS coordinates (required). |
| `Timestamp` | `DateTimeOffset` | Time of the fix (required). |
| `Heading` | `double?` | Compass heading in degrees. |
| `Speed` | `double?` | Speed in km/h. |

## MatchedLink

Each matched road segment:

| Property | Type | Description |
|----------|------|-------------|
| `LinkId` | `string?` | HERE link identifier. |
| `Confidence` | `double?` | Match confidence (0–1). |
| `SpeedLimit` | `double?` | Speed limit in km/h. |
| `FunctionalClass` | `int?` | Road class (1 = motorway, 5 = local). |
| `Geometry` | `List<LatLngLiteral>?` | Road link coordinates for rendering. |

The result also includes a `Warnings` list with any issues encountered during matching.
