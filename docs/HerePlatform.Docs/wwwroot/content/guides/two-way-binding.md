---
title: Two-Way Binding
category: Guides
order: 2
description: "Binding map state to Blazor properties."
---

## @bind Syntax

`AdvancedHereMap` and map object components support Blazor two-way binding using the `@bind-PropertyName` syntax. When the user interacts with the map (panning, zooming, dragging markers), the bound properties update automatically. When you change the bound property in code, the map updates to match.

```xml
<AdvancedHereMap Options="@options"
                 @bind-Center="center"
                 @bind-Zoom="zoom">
</AdvancedHereMap>

<p>Center: @center?.Lat, @center?.Lng | Zoom: @zoom</p>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };

    private LatLngLiteral? center;
    private double zoom = 12;
}
```

## Supported Properties

### AdvancedHereMap

| Property | Type | Callback | Trigger |
|----------|------|----------|---------|
| `Center` | `LatLngLiteral?` | `CenterChanged` | User finishes panning. |
| `Zoom` | `double?` | `ZoomChanged` | User finishes zooming. |
| `Tilt` | `double?` | `TiltChanged` | User finishes tilting. |
| `Heading` | `double?` | `HeadingChanged` | User finishes rotating. |

All map view bindings fire on the `mapviewchangeend` event, not continuously during interaction.

### MarkerComponent

| Property | Type | Callback | Trigger |
|----------|------|----------|---------|
| `Lat` | `double` | `LatChanged` | Marker drag ends. |
| `Lng` | `double` | `LngChanged` | Marker drag ends. |

Requires `Draggable="true"` for drag-based binding.

### CircleComponent

| Property | Type | Callback | Trigger |
|----------|------|----------|---------|
| `CenterLat` | `double` | `CenterLatChanged` | Circle drag ends. |
| `CenterLng` | `double` | `CenterLngChanged` | Circle drag ends. |

## Example

A draggable marker with position bound to component state:

```xml
<AdvancedHereMap Options="@options" @bind-Zoom="currentZoom">
    <MarkerComponent @bind-Lat="lat" @bind-Lng="lng" Draggable="true" />
</AdvancedHereMap>

<p>Marker: @lat.ToString("F4"), @lng.ToString("F4")</p>
<p>Zoom: @currentZoom.ToString("F1")</p>

<button @onclick="ResetPosition">Reset</button>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };

    private double lat = 52.52;
    private double lng = 13.405;
    private double currentZoom = 12;

    private void ResetPosition()
    {
        lat = 52.52;
        lng = 13.405;
    }
}
```

Setting bound properties programmatically pushes changes to the JS map. The `Animate` parameter on `AdvancedHereMap` controls whether programmatic `Center`/`Zoom` changes animate smoothly:

```xml
<AdvancedHereMap Options="@options"
                 Animate="true"
                 @bind-Center="center"
                 @bind-Zoom="zoom">
</AdvancedHereMap>
```
