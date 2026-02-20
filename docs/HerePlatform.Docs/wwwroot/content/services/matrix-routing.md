---
title: Matrix Routing Service
category: Services
order: 7
description: "Calculate distance/time matrices between multiple points."
apiRef: IMatrixRoutingService
---

## Basic Usage

Calculate travel times and distances between multiple origins and destinations in a single request.

```csharp
@inject IMatrixRoutingService MatrixRoutingService
```

```csharp
var request = new MatrixRoutingRequest
{
    Origins =
    [
        new LatLngLiteral(52.52, 13.405),    // Berlin
        new LatLngLiteral(48.8566, 2.3522)   // Paris
    ],
    Destinations =
    [
        new LatLngLiteral(51.5074, -0.1278), // London
        new LatLngLiteral(40.4168, -3.7038)  // Madrid
    ],
    TransportMode = TransportMode.Car
};

var result = await MatrixRoutingService.CalculateMatrixAsync(request);
```

## MatrixRoutingRequest

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Origins` | `List<LatLngLiteral>` | `[]` | Origin positions. |
| `Destinations` | `List<LatLngLiteral>` | `[]` | Destination positions. |
| `TransportMode` | `TransportMode` | `Car` | `Car`, `Truck`, `Pedestrian`, `Bicycle`, `Scooter`, `Taxi`. |
| `RoutingMode` | `RoutingMode` | `Fast` | `Fast` (time) or `Short` (distance). |
| `DepartureTime` | `DateTime?` | `null` | Departure time for time-aware routing. |

```csharp
var request = new MatrixRoutingRequest
{
    Origins = origins,
    Destinations = destinations,
    TransportMode = TransportMode.Truck,
    RoutingMode = RoutingMode.Fast,
    DepartureTime = DateTime.UtcNow
};
```

## Result Handling

`MatrixRoutingResult` contains metadata and a flat list of `MatrixEntry` objects. Each entry maps an origin index to a destination index with duration and distance.

```csharp
var result = await MatrixRoutingService.CalculateMatrixAsync(request);

Console.WriteLine($"Matrix: {result.NumOrigins}x{result.NumDestinations}");

foreach (var entry in result.Matrix)
{
    Console.WriteLine(
        $"Origin {entry.OriginIndex} -> Dest {entry.DestinationIndex}: " +
        $"{entry.Duration / 60} min, {entry.Length / 1000.0:F1} km");
}
```

Each `MatrixEntry` contains:

| Property | Type | Description |
|----------|------|-------------|
| `OriginIndex` | `int` | Index of the origin in the request list. |
| `DestinationIndex` | `int` | Index of the destination in the request list. |
| `Duration` | `int` | Travel duration in seconds. |
| `Length` | `int` | Travel distance in meters. |

To build a 2D matrix from the flat list:

```csharp
var matrix = new int[result.NumOrigins, result.NumDestinations];
foreach (var entry in result.Matrix)
{
    matrix[entry.OriginIndex, entry.DestinationIndex] = entry.Duration;
}
```
