---
title: Waypoint Sequence Service
category: Services
order: 8
description: "Optimize waypoint ordering."
apiRef: IWaypointSequenceService
---

## Basic Usage

Optimize the order of waypoints to minimize total travel time or distance. Useful for delivery route planning and multi-stop trip optimization.

```csharp
@inject IWaypointSequenceService WaypointSequenceService
```

```csharp
var request = new WaypointSequenceRequest
{
    Start = new LatLngLiteral(52.52, 13.405),
    End = new LatLngLiteral(52.52, 13.405),  // Round-trip
    Waypoints =
    [
        new LatLngLiteral(52.53, 13.42),
        new LatLngLiteral(52.50, 13.39),
        new LatLngLiteral(52.51, 13.45)
    ],
    TransportMode = TransportMode.Car
};

var result = await WaypointSequenceService.OptimizeSequenceAsync(request);
```

## Request

`WaypointSequenceRequest` defines the fixed start and end points and the waypoints to reorder.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Start` | `LatLngLiteral` | -- | Fixed start position (required). |
| `End` | `LatLngLiteral` | -- | Fixed end position (required). Use the same value as `Start` for round-trips. |
| `Waypoints` | `List<LatLngLiteral>?` | `null` | Waypoints to optimize the sequence for. |
| `TransportMode` | `TransportMode` | `Car` | `Car`, `Truck`, `Pedestrian`, `Bicycle`, `Scooter`. |

```csharp
// One-way trip with different start and end
var request = new WaypointSequenceRequest
{
    Start = new LatLngLiteral(48.8566, 2.3522),   // Paris
    End = new LatLngLiteral(52.52, 13.405),         // Berlin
    Waypoints =
    [
        new LatLngLiteral(50.1109, 8.6821),  // Frankfurt
        new LatLngLiteral(50.9375, 6.9603),  // Cologne
        new LatLngLiteral(49.4521, 11.0767)  // Nuremberg
    ],
    TransportMode = TransportMode.Car
};
```

## Optimized Result

`WaypointSequenceResult` provides the reordered waypoints along with total travel metrics.

| Property | Type | Description |
|----------|------|-------------|
| `OptimizedIndices` | `List<int>?` | Original waypoint indices in optimized order. |
| `OptimizedWaypoints` | `List<LatLngLiteral>?` | Waypoints in the recommended order. |
| `TotalDistance` | `int` | Total distance in meters. |
| `TotalDuration` | `int` | Total duration in seconds. |

```csharp
var result = await WaypointSequenceService.OptimizeSequenceAsync(request);

Console.WriteLine($"Total: {result.TotalDuration / 60} min, " +
    $"{result.TotalDistance / 1000.0:F1} km");

// Iterate in optimized order
foreach (var wp in result.OptimizedWaypoints ?? [])
{
    Console.WriteLine($"  Stop: {wp.Lat}, {wp.Lng}");
}

// Map original indices to see how they were reordered
foreach (var idx in result.OptimizedIndices ?? [])
{
    Console.WriteLine($"  Original index: {idx}");
}
```
