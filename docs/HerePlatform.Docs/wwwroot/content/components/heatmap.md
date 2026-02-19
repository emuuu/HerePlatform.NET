---
title: HeatmapComponent
category: Components
order: 13
description: "Heatmap visualization for geographic data density."
apiRef: HeatmapComponent
demo: map-heatmap
---

## Basic Usage

`HeatmapComponent` renders a heatmap layer on the map. Place it inside an `AdvancedHereMap` and provide a list of `HeatmapDataPoint` objects.

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Data

<AdvancedHereMap Options="_options" Height="500px">
    <HeatmapComponent DataPoints="_points" />
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 11
    };

    private List<HeatmapDataPoint> _points = new()
    {
        new(52.52, 13.405, 1.0),
        new(52.53, 13.41, 0.8),
        new(52.51, 13.39, 1.5),
        new(52.525, 13.40, 2.0)
    };
}
```

## Data Points

Each `HeatmapDataPoint` has `Lat`, `Lng`, and a `Value` that controls the intensity at that location. Higher values produce a stronger heat signal.

```csharp
var points = new List<HeatmapDataPoint>
{
    new() { Lat = 48.8566, Lng = 2.3522, Value = 5.0 },
    new() { Lat = 48.8584, Lng = 2.2945, Value = 3.0 },
    new() { Lat = 48.8530, Lng = 2.3499, Value = 1.0 },
};
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Lat` | `double` | -- | Latitude of the data point. |
| `Lng` | `double` | -- | Longitude of the data point. |
| `Value` | `double` | `1` | Intensity value (higher = more heat). |

Update the `DataPoints` list and the heatmap re-renders automatically on the next parameter change cycle.

## Customization

Control appearance with `Opacity`, `Colors` (gradient stops), `SampleDepth`, and `Visible`.

```csharp
<HeatmapComponent DataPoints="_points"
                  Opacity="0.8"
                  Colors="_gradient"
                  SampleDepth="8"
                  Visible="@_showHeatmap" />

@code {
    private bool _showHeatmap = true;

    private Dictionary<double, string> _gradient = new()
    {
        [0.0] = "rgba(0, 0, 255, 0)",
        [0.3] = "blue",
        [0.5] = "green",
        [0.7] = "yellow",
        [1.0] = "red"
    };
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `DataPoints` | `List<HeatmapDataPoint>?` | `null` | Data points for the heatmap. |
| `Opacity` | `double` | `0.6` | Layer opacity from 0 (transparent) to 1 (opaque). |
| `Colors` | `Dictionary<double, string>?` | `null` | Gradient color stops. Keys are 0--1 thresholds, values are CSS colors. |
| `SampleDepth` | `int?` | `null` | Sampling depth for the heatmap calculation. |
| `Visible` | `bool` | `true` | Controls layer visibility. |

Toggle `Visible` to show or hide the heatmap without removing data. Combine with a `GroupComponent` to layer multiple heatmaps and toggle them independently.
