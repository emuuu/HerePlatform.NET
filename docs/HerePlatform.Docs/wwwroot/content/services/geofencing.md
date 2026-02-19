---
title: Geofencing Service
category: Services
order: 9
description: "Check positions against geofence zones."
apiRef: IGeofencingService
---

## Basic Usage

Check whether a geographic position falls inside one or more geofence zones. The service performs client-side point-in-polygon and point-in-circle checks.

```csharp
@inject IGeofencingService GeofencingService
```

```csharp
var position = new LatLngLiteral(52.52, 13.405);

var zones = new List<GeofenceZone>
{
    new GeofenceZone
    {
        Id = "zone-1",
        Name = "Downtown Berlin",
        Type = "circle",
        Center = new LatLngLiteral(52.52, 13.405),
        Radius = 5000 // meters
    }
};

var result = await GeofencingService.CheckPositionAsync(position, zones);

if (result.IsInside)
{
    Console.WriteLine("Inside zones: " +
        string.Join(", ", result.MatchedZoneIds ?? []));
}
```

## Geofence Zones

Zones can be defined as circles or polygons.

**Circle zone:**

```csharp
var circleZone = new GeofenceZone
{
    Id = "delivery-area",
    Name = "Delivery Zone",
    Type = "circle",
    Center = new LatLngLiteral(52.52, 13.405),
    Radius = 10000  // 10 km
};
```

**Polygon zone:**

```csharp
var polygonZone = new GeofenceZone
{
    Id = "restricted-area",
    Name = "Restricted Area",
    Type = "polygon",
    Vertices =
    [
        new LatLngLiteral(52.53, 13.38),
        new LatLngLiteral(52.53, 13.42),
        new LatLngLiteral(52.50, 13.42),
        new LatLngLiteral(52.50, 13.38)
    ]
};
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Id` | `string?` | `null` | Unique identifier for the zone. |
| `Name` | `string?` | `null` | Display name. |
| `Type` | `string` | `"polygon"` | Zone type: `"polygon"` or `"circle"`. |
| `Vertices` | `List<LatLngLiteral>?` | `null` | Polygon vertices (for polygon type). |
| `Center` | `LatLngLiteral?` | `null` | Center position (for circle type). |
| `Radius` | `double` | `0` | Radius in meters (for circle type). |

## Check Results

`GeofenceCheckResult` indicates whether the position is inside any zone and which zones matched.

| Property | Type | Description |
|----------|------|-------------|
| `IsInside` | `bool` | `true` if the position is inside at least one zone. |
| `MatchedZoneIds` | `List<string>?` | IDs of all zones the position is inside. |

```csharp
var zones = new List<GeofenceZone> { circleZone, polygonZone };
var result = await GeofencingService.CheckPositionAsync(position, zones);

if (!result.IsInside)
{
    Console.WriteLine("Position is outside all geofence zones.");
}
```
