---
title: ZoomRectangleComponent
category: Components
order: 22
description: "Zoom-to-rectangle selection tool."
apiRef: ZoomRectangleComponent
---

## Basic Usage

`ZoomRectangleComponent` lets users draw a rectangle on the map to zoom into that area. When active, the user clicks and drags to define a bounding box, and the map zooms to fit the selected region.

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.UI

<AdvancedHereMap Options="_options" Height="500px">
    <ZoomRectangleComponent Active="@_zoomRectActive"
                            Alignment="UIAlignment.TopRight" />
</AdvancedHereMap>

<button @onclick="() => _zoomRectActive = !_zoomRectActive">
    @(_zoomRectActive ? "Cancel Zoom Rect" : "Zoom to Rectangle")
</button>

@code {
    private bool _zoomRectActive;

    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 10
    };
}
```

When `Active` is set to `true`, normal map panning is temporarily suspended and the cursor changes to a crosshair. The user draws a rectangle by clicking and dragging. After release, the map animates to fill the selected area, and the tool can be kept active for repeated use or deactivated by setting `Active` back to `false`.

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Active` | `bool` | `false` | Activates or deactivates the zoom rectangle tool. |
| `Alignment` | `UIAlignment` | `TopRight` | Position of the UI control on the map. |

### Alignment Options

| Value | Position |
|-------|----------|
| `TopLeft` | Top-left corner |
| `TopCenter` | Top center |
| `TopRight` | Top-right corner (default) |
| `BottomLeft` | Bottom-left corner |
| `BottomCenter` | Bottom center |
| `BottomRight` | Bottom-right corner |
| `LeftTop` | Left side, near top |
| `LeftMiddle` | Left side, middle |
| `LeftBottom` | Left side, near bottom |
| `RightTop` | Right side, near top |
| `RightMiddle` | Right side, middle |
| `RightBottom` | Right side, near bottom |

The component must be a child of `AdvancedHereMap`. It is disposed automatically when the parent map is removed.
