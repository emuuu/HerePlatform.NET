---
title: DistanceMeasurementComponent
category: Components
order: 20
description: "Interactive distance measurement tool."
apiRef: DistanceMeasurementComponent
---

## Basic Usage

`DistanceMeasurementComponent` adds an interactive distance measurement tool to the map. Users click points on the map to measure distances between them.

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.UI

<AdvancedHereMap Options="_options" Height="500px">
    <DistanceMeasurementComponent Active="@_measuring"
                                  OnMeasured="HandleMeasured" />
</AdvancedHereMap>

<button @onclick="() => _measuring = !_measuring">
    @(_measuring ? "Stop Measuring" : "Start Measuring")
</button>

@if (_lastDistance > 0)
{
    <p>Distance: @(_lastDistance / 1000.0):F2 km</p>
}

@code {
    private bool _measuring;
    private double _lastDistance;

    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };

    private void HandleMeasured(double distanceMeters)
    {
        _lastDistance = distanceMeters;
    }
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Active` | `bool` | `false` | Activates or deactivates the measurement tool. |
| `Alignment` | `UIAlignment` | `TopRight` | Position of the UI control on the map. |
| `OnMeasured` | `EventCallback<double>` | -- | Fires when measurement completes, with distance in meters. |

## Integration with AdvancedHereMap

The component must be a child of `AdvancedHereMap`. It registers as a UI control and appears at the position specified by `Alignment`.

```csharp
<AdvancedHereMap Options="_options" Height="500px">
    <DistanceMeasurementComponent Active="true"
                                  Alignment="UIAlignment.BottomLeft" />
</AdvancedHereMap>
```

Available alignment positions:

| Value | Position |
|-------|----------|
| `TopLeft` | Top-left corner |
| `TopCenter` | Top center |
| `TopRight` | Top-right corner (default) |
| `LeftTop` | Left side, near top |
| `LeftMiddle` | Left side, middle |
| `LeftBottom` | Left side, near bottom |
| `RightTop` | Right side, near top |
| `RightMiddle` | Right side, middle |
| `RightBottom` | Right side, near bottom |
| `BottomLeft` | Bottom-left corner |
| `BottomCenter` | Bottom center |
| `BottomRight` | Bottom-right corner |

Toggle `Active` to enable or disable the measurement mode programmatically. When deactivated, any in-progress measurement is cancelled.
