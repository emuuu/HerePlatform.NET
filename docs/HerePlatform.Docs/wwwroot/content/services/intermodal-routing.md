---
title: Intermodal Routing Service
category: Services
order: 15
description: "Route combining transit and walking segments."
apiRef: IIntermodalRoutingService
---

## Setup

Inject `IIntermodalRoutingService` into your component. It is registered automatically by `AddBlazorHerePlatform`.

```csharp
@inject IIntermodalRoutingService IntermodalRoutingService
```

## CalculateRouteAsync

Calculate routes that combine public transit and pedestrian segments. Returns multi-section routes with departure/arrival times, transit line details, and polylines.

```csharp
var result = await IntermodalRoutingService.CalculateRouteAsync(new IntermodalRoutingRequest
{
    Origin = new LatLngLiteral(52.5200, 13.4050),
    Destination = new LatLngLiteral(52.5400, 13.3600),
    DepartAt = "2025-01-15T08:00:00",
    Lang = "de"
});

foreach (var route in result.Routes ?? [])
{
    foreach (var section in route.Sections ?? [])
    {
        var dep = section.Departure;
        var arr = section.Arrival;
        Console.WriteLine($"[{section.Type}] {dep?.Name} → {arr?.Name}");
        Console.WriteLine($"  {dep?.Time} → {arr?.Time}");

        if (section.Transport is { } t && t.Mode != "pedestrian")
            Console.WriteLine($"  Line: {t.ShortName} ({t.Mode}) → {t.Headsign}");

        if (section.Summary is { } s)
            Console.WriteLine($"  {s.Duration / 60} min, {s.Length} m");
    }
}
```

## IntermodalRoutingRequest Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Origin` | `LatLngLiteral` | -- | Start position (required). |
| `Destination` | `LatLngLiteral` | -- | End position (required). |
| `DepartAt` | `string?` | `null` | Departure time (ISO 8601). |
| `ArriveAt` | `string?` | `null` | Desired arrival time (ISO 8601). |
| `Alternatives` | `int` | `0` | Number of alternative routes. |
| `ReturnPolyline` | `bool` | `true` | Include polyline geometry. |
| `ReturnActions` | `bool` | `false` | Include turn-by-turn actions. |
| `ReturnTravelSummary` | `bool` | `true` | Include duration and distance. |
| `Lang` | `string?` | `null` | Language for place names. |

Either `DepartAt` or `ArriveAt` may be set, but not both.

## IntermodalSection

Each route consists of alternating pedestrian and transit sections:

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `string?` | `"pedestrian"` or `"transit"`. |
| `Departure` | `IntermodalPlace?` | Departure stop/place with name, position, time. |
| `Arrival` | `IntermodalPlace?` | Arrival stop/place with name, position, time. |
| `Summary` | `IntermodalSummary?` | Duration (seconds) and length (meters). |
| `Transport` | `IntermodalTransport?` | Transport mode and line details. |
| `Polyline` | `string?` | Encoded flexible polyline. |
| `Geometry` | `List<LatLngLiteral>?` | Decoded polyline coordinates. |

## IntermodalTransport

Transit details for non-pedestrian sections:

| Property | Type | Description |
|----------|------|-------------|
| `Mode` | `string?` | `"subway"`, `"bus"`, `"tram"`, `"train"`, etc. |
| `Name` | `string?` | Full line name (e.g. `"U6"`). |
| `ShortName` | `string?` | Short line name. |
| `Headsign` | `string?` | Direction/terminus (e.g. `"Alt-Tegel"`). |
| `Color` | `string?` | Line color as hex (e.g. `"#7B3F98"`). |
