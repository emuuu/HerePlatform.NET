---
title: Traffic Service
category: Services
order: 3
description: "Real-time traffic flow and incidents."
apiRef: ITrafficService
demo: map-traffic
---

## Traffic Flow

Retrieve real-time traffic flow data within a bounding box. Flow data includes current speeds, free-flow speeds, and jam factors.

```csharp
@inject ITrafficService TrafficService
```

```csharp
var flow = await TrafficService.GetTrafficFlowAsync(
    north: 52.55, south: 52.50, east: 13.45, west: 13.35);

foreach (var item in flow.Items ?? [])
{
    Console.WriteLine($"{item.RoadName}: {item.CurrentSpeed} km/h");
    Console.WriteLine($"Free flow: {item.FreeFlowSpeed} km/h");
    Console.WriteLine($"Jam factor: {item.JamFactor}");
}
```

Each `TrafficFlowItem` contains:

| Property | Type | Description |
|----------|------|-------------|
| `CurrentSpeed` | `double` | Current speed in km/h. |
| `FreeFlowSpeed` | `double` | Speed without traffic in km/h. |
| `JamFactor` | `double` | Congestion level: 0 = free flow, 10 = standstill. |
| `RoadName` | `string?` | Name of the road. |
| `Position` | `LatLngLiteral?` | Location along the road. |

## Traffic Incidents

Retrieve traffic incidents (accidents, construction, congestion) within a bounding box.

```csharp
var incidents = await TrafficService.GetTrafficIncidentsAsync(
    north: 52.55, south: 52.50, east: 13.45, west: 13.35);

foreach (var incident in incidents.Incidents ?? [])
{
    Console.WriteLine($"[{incident.Type}] {incident.Description}");
    Console.WriteLine($"Severity: {incident.Severity}");
    Console.WriteLine($"Road: {incident.RoadName}");
    Console.WriteLine($"From: {incident.StartTime} To: {incident.EndTime}");
}
```

Each `TrafficIncident` contains:

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `string?` | Incident type: `"accident"`, `"construction"`, `"congestion"`. |
| `Severity` | `int` | Severity level: 0 = unknown, 4 = blocking. |
| `Description` | `string?` | Human-readable description. |
| `Position` | `LatLngLiteral?` | Location of the incident. |
| `RoadName` | `string?` | Road where the incident occurs. |
| `StartTime` | `string?` | Start time (ISO 8601). |
| `EndTime` | `string?` | End time (ISO 8601). |

Both methods accept a bounding box defined by four `double` parameters: `north`, `south`, `east`, `west`. Use the current map viewport bounds to query traffic data for the visible area.
