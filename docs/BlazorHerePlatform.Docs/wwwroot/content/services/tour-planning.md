---
title: Tour Planning Service
category: Services
order: 16
description: "Optimize vehicle tours for deliveries and pickups."
apiRef: ITourPlanningService
---

## Setup

Inject `ITourPlanningService` into your component. It is registered automatically by `AddBlazorHerePlatform`.

```csharp
@inject ITourPlanningService TourPlanningService
```

## SolveAsync

Solve a vehicle routing problem with jobs (deliveries/pickups) and a fleet of vehicles. The optimizer assigns jobs to vehicles and determines the optimal stop sequence.

```csharp
var problem = new TourPlanningProblem
{
    Plan = new TourPlan
    {
        Jobs =
        [
            new TourJob
            {
                Id = "delivery-1",
                Places = new TourJobPlaces
                {
                    Deliveries =
                    [
                        new TourJobPlace
                        {
                            Location = new LatLngLiteral(52.5163, 13.3777),
                            Duration = 300
                        }
                    ]
                }
            },
            new TourJob
            {
                Id = "delivery-2",
                Places = new TourJobPlaces
                {
                    Deliveries =
                    [
                        new TourJobPlace
                        {
                            Location = new LatLngLiteral(52.5200, 13.4050),
                            Duration = 600
                        }
                    ]
                }
            }
        ]
    },
    Fleet = new TourFleet
    {
        Types =
        [
            new TourVehicleType
            {
                Id = "van-1",
                Profile = "car",
                Amount = 1,
                Capacity = [20],
                Costs = new TourVehicleCosts { Fixed = 10, Distance = 0.001, Time = 0.01 },
                Shifts =
                [
                    new TourVehicleShift
                    {
                        Start = new TourShiftEnd
                        {
                            Location = new LatLngLiteral(52.53, 13.38),
                            Time = "2025-01-15T08:00:00Z"
                        },
                        End = new TourShiftEnd
                        {
                            Location = new LatLngLiteral(52.53, 13.38),
                            Time = "2025-01-15T18:00:00Z"
                        }
                    }
                ]
            }
        ]
    }
};

var result = await TourPlanningService.SolveAsync(problem);

foreach (var tour in result.Tours ?? [])
{
    Console.WriteLine($"Vehicle: {tour.VehicleId}");
    Console.WriteLine($"  Cost: {tour.Statistic?.Cost}, Distance: {tour.Statistic?.Distance} m");
    foreach (var stop in tour.Stops ?? [])
    {
        foreach (var act in stop.Activities ?? [])
            Console.WriteLine($"  [{act.Type}] {act.JobId}");
    }
}

if (result.UnassignedJobs is { Count: > 0 } unassigned)
    Console.WriteLine($"Unassigned: {string.Join(", ", unassigned)}");
```

## Problem Structure

### TourPlan

| Property | Type | Description |
|----------|------|-------------|
| `Jobs` | `List<TourJob>?` | Jobs to be assigned to vehicles. |

### TourJob

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string?` | Unique job identifier. |
| `Places` | `TourJobPlaces?` | Delivery and/or pickup locations. |

### TourJobPlace

| Property | Type | Description |
|----------|------|-------------|
| `Location` | `LatLngLiteral` | Job location (required). |
| `Duration` | `int` | Service time in seconds. |
| `Times` | `List<List<string>>?` | Time windows as `[start, end]` ISO 8601 pairs. |

### TourFleet

| Property | Type | Description |
|----------|------|-------------|
| `Types` | `List<TourVehicleType>?` | Vehicle type definitions. |

### TourVehicleType

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string?` | Vehicle type identifier. |
| `Profile` | `string?` | Routing profile (`"car"`, `"truck"`). |
| `Costs` | `TourVehicleCosts?` | Cost parameters. |
| `Shifts` | `List<TourVehicleShift>?` | Work shifts with start/end locations and times. |
| `Capacity` | `List<int>?` | Vehicle capacity dimensions. |
| `Amount` | `int` | Number of vehicles of this type. |

### TourVehicleCosts

| Property | Type | Description |
|----------|------|-------------|
| `Fixed` | `double` | Fixed cost per vehicle. |
| `Distance` | `double` | Cost per meter. |
| `Time` | `double` | Cost per second. |

## Result Structure

`TourPlanningResult` contains:

| Property | Type | Description |
|----------|------|-------------|
| `Tours` | `List<Tour>?` | Planned vehicle tours. |
| `UnassignedJobs` | `List<string>?` | Job IDs that could not be assigned. |
| `Statistic` | `TourStatistic?` | Overall cost, distance, and duration. |

Each `Tour` includes `VehicleId`, a list of `TourStop` entries, and a `TourStatistic`. Each stop contains `Activities` with `Type` (`"departure"`, `"delivery"`, `"pickup"`, `"arrival"`) and `JobId`.
