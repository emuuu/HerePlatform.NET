---
title: OverviewMapComponent
category: Components
order: 21
description: "Mini overview map showing the current viewport."
apiRef: OverviewMapComponent
---

## Basic Usage

`OverviewMapComponent` displays a small overview (mini-map) that shows the main map's current viewport at a wider zoom level. It automatically tracks the main map's center position.

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.UI

<AdvancedHereMap Options="_options" Height="500px">
    <OverviewMapComponent />
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 14
    };
}
```

By default, the overview map appears in the bottom-right corner and displays the area at a zoom level 4 steps lower than the main map.

### Customization

Control the position and zoom difference:

```csharp
<OverviewMapComponent Alignment="UIAlignment.BottomLeft"
                      ZoomDelta="6" />
```

A larger `ZoomDelta` shows more surrounding area in the overview.

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Alignment` | `UIAlignment` | `BottomRight` | Position of the mini-map on the main map. |
| `ZoomDelta` | `int` | `4` | Zoom level difference between the main map and the overview. |

### Alignment Options

| Value | Position |
|-------|----------|
| `TopLeft` | Top-left corner |
| `TopCenter` | Top center |
| `TopRight` | Top-right corner |
| `BottomLeft` | Bottom-left corner |
| `BottomCenter` | Bottom center |
| `BottomRight` | Bottom-right corner (default) |
| `LeftTop` | Left side, near top |
| `LeftMiddle` | Left side, middle |
| `LeftBottom` | Left side, near bottom |
| `RightTop` | Right side, near top |
| `RightMiddle` | Right side, middle |
| `RightBottom` | Right side, near bottom |

The overview map component must be placed as a child of `AdvancedHereMap`. It is disposed automatically when the parent map is removed.
