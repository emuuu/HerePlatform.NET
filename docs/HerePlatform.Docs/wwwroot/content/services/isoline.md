---
title: Isoline Service
category: Services
order: 6
description: "Calculate isochrone and isodistance areas."
apiRef: IIsolineService
---

## Isochrone

Calculate the area reachable from a center point within a given time. Ranges are specified in seconds.

```csharp
@inject IIsolineService IsolineService
```

```csharp
var request = new IsolineRequest
{
    Center = new LatLngLiteral(52.52, 13.405),
    Ranges = [300, 600, 900],  // 5, 10, 15 minutes
    RangeType = IsolineRangeType.Time,
    TransportMode = TransportMode.Car
};

var result = await IsolineService.CalculateIsolineAsync(request);

foreach (var isoline in result.Isolines ?? [])
{
    Console.WriteLine($"Range: {isoline.Range}s, Points: {isoline.Polygon?.Count}");
}
```

## Isodistance

Calculate the area reachable within a given distance. Ranges are specified in meters.

```csharp
var request = new IsolineRequest
{
    Center = new LatLngLiteral(52.52, 13.405),
    Ranges = [1000, 3000, 5000],  // 1km, 3km, 5km
    RangeType = IsolineRangeType.Distance,
    TransportMode = TransportMode.Pedestrian
};

var result = await IsolineService.CalculateIsolineAsync(request);

foreach (var isoline in result.Isolines ?? [])
{
    // Use Polygon coordinates to render on the map
    var coords = isoline.Polygon; // List<LatLngLiteral>
}
```

## IsolineRequest Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Center` | `LatLngLiteral` | -- | Center position (required). |
| `Ranges` | `List<int>?` | `null` | Range values (seconds for time, meters for distance). |
| `RangeType` | `IsolineRangeType` | `Time` | `Time`, `Distance`, or `Consumption`. |
| `TransportMode` | `TransportMode` | `Car` | `Car`, `Truck`, `Pedestrian`, `Bicycle`, `Scooter`. |
| `RoutingMode` | `RoutingMode` | `Fast` | `Fast` or `Short`. |
| `Avoid` | `RoutingAvoidFeature` | `None` | Features to avoid: `Tolls`, `Highways`, `Ferries`, `Tunnels`. |
| `DepartureTime` | `string?` | `null` | Departure time in ISO 8601 format. |

Each `IsolinePolygon` in the result contains:

| Property | Type | Description |
|----------|------|-------------|
| `Range` | `int` | The range value for this isoline. |
| `Polygon` | `List<LatLngLiteral>?` | Decoded polygon coordinates. |
| `EncodedPolyline` | `string?` | Encoded flexible polyline (for manual decoding). |

```csharp
var request = new IsolineRequest
{
    Center = new LatLngLiteral(48.8566, 2.3522),
    Ranges = [600],
    RangeType = IsolineRangeType.Time,
    TransportMode = TransportMode.Bicycle,
    Avoid = RoutingAvoidFeature.Tunnels,
    DepartureTime = "2025-06-15T08:00:00Z"
};
```
