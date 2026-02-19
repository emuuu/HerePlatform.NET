---
title: Public Transit Service
category: Services
order: 4
description: "Public transit departures and stations."
apiRef: IPublicTransitService
---

## Departures

Retrieve upcoming departures from the nearest transit station to a given position.

```csharp
@inject IPublicTransitService TransitService
```

```csharp
var position = new LatLngLiteral(52.5200, 13.4050);
var result = await TransitService.GetDeparturesAsync(position);

foreach (var departure in result.Departures ?? [])
{
    Console.WriteLine($"{departure.LineName} -> {departure.Direction}");
    Console.WriteLine($"Type: {departure.TransportType}");
    Console.WriteLine($"Departs: {departure.DepartureTime}");
    Console.WriteLine($"Station: {departure.StationName}");
}
```

Each `TransitDeparture` contains:

| Property | Type | Description |
|----------|------|-------------|
| `LineName` | `string?` | Line name or number (e.g. `"U2"`, `"Bus 100"`). |
| `Direction` | `string?` | Destination of the line. |
| `DepartureTime` | `string?` | Departure time (ISO 8601). |
| `TransportType` | `string?` | Transport type: `"bus"`, `"subway"`, `"train"`, `"tram"`. |
| `StationName` | `string?` | Name of the departure station. |

## Stations

Search for transit stations near a position within a given radius.

```csharp
var position = new LatLngLiteral(52.5200, 13.4050);
var result = await TransitService.SearchStationsAsync(position, radiusMeters: 1000);

foreach (var station in result.Stations ?? [])
{
    Console.WriteLine($"{station.Name} ({station.Distance:F0}m)");
    Console.WriteLine($"Position: {station.Position?.Lat}, {station.Position?.Lng}");
    Console.WriteLine($"Types: {string.Join(", ", station.TransportTypes ?? [])}");
}
```

Each `TransitStation` contains:

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string?` | Station name. |
| `Position` | `LatLngLiteral?` | Station coordinates. |
| `Distance` | `double` | Distance from search center in meters. |
| `TransportTypes` | `List<string>?` | Available transport types at this station. |

The `radiusMeters` parameter defaults to `500` meters when omitted:

```csharp
// Default 500m radius
var nearby = await TransitService.SearchStationsAsync(position);

// Custom 2km radius
var wider = await TransitService.SearchStationsAsync(position, 2000);
```
