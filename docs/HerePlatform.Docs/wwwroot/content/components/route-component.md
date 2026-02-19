---
title: RouteComponent
category: Components
order: 18
description: "Display calculated routes on the map."
demo: map-routing
---

## Basic Usage

`RouteComponent` calculates a route between an origin and destination using the HERE Routing API, then renders it as a polyline on the map. It requires `IRoutingService` to be registered:

```csharp
builder.Services.AddBlazorHerePlatform("YOUR_KEY");
```

Place it inside an `AdvancedHereMap`:

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Services.Routing

<AdvancedHereMap Options="_options" Height="500px">
    <RouteComponent Origin="new LatLngLiteral(52.52, 13.405)"
                    Destination="new LatLngLiteral(48.8566, 2.3522)"
                    OnRouteCalculated="HandleRoute" />
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(50.0, 8.0),
        Zoom = 6
    };

    private void HandleRoute(RoutingResult result)
    {
        var section = result.Routes?[0]?.Sections?[0]?.Summary;
        if (section is not null)
        {
            Console.WriteLine($"Distance: {section.Length / 1000.0:F1} km");
            Console.WriteLine($"Duration: {section.Duration / 60} min");
        }
    }
}
```

## Route Visualization

Customize the route line appearance with `StrokeColor` and `LineWidth`. Add via waypoints with the `Via` parameter.

```csharp
<RouteComponent Origin="_origin"
                Destination="_destination"
                Via="_viaPoints"
                StrokeColor="#00AA44"
                LineWidth="6"
                TransportMode="TransportMode.Pedestrian"
                RoutingMode="RoutingMode.Short"
                Visible="@_showRoute" />

@code {
    private LatLngLiteral _origin = new(52.52, 13.405);
    private LatLngLiteral _destination = new(52.50, 13.45);

    private List<LatLngLiteral> _viaPoints = new()
    {
        new(52.51, 13.42)
    };

    private bool _showRoute = true;
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Origin` | `LatLngLiteral` | -- | Route start position. |
| `Destination` | `LatLngLiteral` | -- | Route end position. |
| `Via` | `List<LatLngLiteral>?` | `null` | Optional intermediate waypoints. |
| `TransportMode` | `TransportMode` | `Car` | Transport mode (Car, Pedestrian, Truck, etc.). |
| `RoutingMode` | `RoutingMode` | `Fast` | Optimization mode (Fast or Short). |
| `Avoid` | `RoutingAvoidFeature` | `None` | Features to avoid (tolls, highways, ferries). |
| `StrokeColor` | `string` | `#0066FF` | Polyline color in CSS format. |
| `LineWidth` | `double` | `5` | Polyline width in pixels. |
| `Visible` | `bool` | `true` | Controls polyline visibility. |
| `OnRouteCalculated` | `EventCallback<RoutingResult>` | -- | Fires when the route is calculated. |
| `OnError` | `EventCallback<Exception>` | -- | Fires if route calculation fails. |

## Combined with Routing Service

For more control, use `IRoutingService` directly and render the result with a `PolylineComponent`. The `RouteComponent` is a convenience wrapper that combines both steps.

```csharp
@inject IRoutingService RoutingService

@code {
    private async Task CalculateManually()
    {
        var result = await RoutingService.CalculateRouteAsync(new RoutingRequest
        {
            Origin = new LatLngLiteral(52.52, 13.405),
            Destination = new LatLngLiteral(48.8566, 2.3522),
            TransportMode = TransportMode.Car,
            ReturnPolyline = true
        });

        // Access result.Routes[0].Sections for polyline data and summaries
    }
}
```

The `RouteComponent.Result` property and `IsCalculating` flag are available on the component reference for status checks.
