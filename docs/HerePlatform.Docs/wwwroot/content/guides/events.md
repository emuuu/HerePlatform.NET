---
title: Events
category: Guides
order: 3
description: "Handling map and object events."
---

## Map Events (OnClick, OnDrag, OnMapViewChange, OnResize)

`AdvancedHereMap` exposes map-level events as Blazor `EventCallback` parameters. These fire when the user interacts with the map background (not on map objects).

```xml
<AdvancedHereMap Options="@options"
                 OnClick="@HandleClick"
                 OnDragEnd="@HandleDragEnd"
                 OnMapViewChangeEnd="@HandleViewChange"
                 OnResize="@HandleResize">
</AdvancedHereMap>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };

    private void HandleClick(MapPointerEventArgs e)
    {
        Console.WriteLine($"Clicked at: {e.Position?.Lat}, {e.Position?.Lng}");
    }

    private void HandleDragEnd(MapDragEventArgs e)
    {
        Console.WriteLine($"Drag ended at: {e.Position?.Lat}, {e.Position?.Lng}");
    }

    private void HandleViewChange(MapViewChangeEventArgs e)
    {
        Console.WriteLine($"View: center={e.Center}, zoom={e.Zoom}");
    }

    private void HandleResize(MapResizeEventArgs e)
    {
        Console.WriteLine($"Resized to: {e.Width}x{e.Height}");
    }
}
```

**All map-level pointer events:**

| Event | EventArgs | Description |
|-------|-----------|-------------|
| `OnClick` | `MapPointerEventArgs` | Tap/click on the map background. |
| `OnDoubleClick` | `MapPointerEventArgs` | Double-tap/double-click. |
| `OnLongPress` | `MapPointerEventArgs` | Long press. |
| `OnContextMenu` | `MapPointerEventArgs` | Right-click. |
| `OnPointerDown` | `MapPointerEventArgs` | Pointer touches the map. |
| `OnPointerUp` | `MapPointerEventArgs` | Pointer leaves the surface. |
| `OnPointerMove` | `MapPointerEventArgs` | Pointer moves over the map. |
| `OnDragStart` | `MapDragEventArgs` | Drag operation starts. |
| `OnDrag` | `MapDragEventArgs` | Continuous during drag. |
| `OnDragEnd` | `MapDragEventArgs` | Drag operation ends. |
| `OnMapViewChange` | `MapViewChangeEventArgs` | Continuous during view change. |
| `OnMapViewChangeStart` | `MapViewChangeEventArgs` | View change starts. |
| `OnMapViewChangeEnd` | `MapViewChangeEventArgs` | View change ends. |
| `OnResize` | `MapResizeEventArgs` | Map container resized. |
| `OnBaseLayerChange` | `BaseLayerChangeEventArgs` | Base layer changed. |

## Marker Events

All map object components (`MarkerComponent`, `PolygonComponent`, `PolylineComponent`, `CircleComponent`, `RectComponent`) inherit the same event set from `MapObjectComponentBase`.

```xml
<MarkerComponent Lat="52.52" Lng="13.405"
                 Clickable="true"
                 Draggable="true"
                 OnClick="@HandleMarkerClick"
                 OnDragEnd="@HandleMarkerDrag" />

@code {
    private void HandleMarkerClick(MapPointerEventArgs e)
    {
        Console.WriteLine("Marker clicked");
    }

    private void HandleMarkerDrag(MapDragEventArgs e)
    {
        Console.WriteLine($"Dragged to: {e.Position?.Lat}, {e.Position?.Lng}");
    }
}
```

## Shape Events

Polygon, polyline, circle, and rect components support the same events as markers. Set `Clickable="true"` or `Draggable="true"` to enable interaction.

```xml
<PolygonComponent Path="@polygonPath"
                  Clickable="true"
                  OnClick="@HandleShapeClick"
                  OnPointerEnter="@HandleEnter"
                  OnPointerLeave="@HandleLeave" />
```

**Shared object-level events:**

| Event | EventArgs | Description |
|-------|-----------|-------------|
| `OnClick` | `MapPointerEventArgs` | Tap/click on the object. |
| `OnDoubleClick` | `MapPointerEventArgs` | Double-tap. |
| `OnLongPress` | `MapPointerEventArgs` | Long press. |
| `OnContextMenu` | `MapPointerEventArgs` | Right-click. |
| `OnPointerEnter` | `MapPointerEventArgs` | Pointer enters the object. |
| `OnPointerLeave` | `MapPointerEventArgs` | Pointer leaves the object. |
| `OnDragStart` | `MapDragEventArgs` | Drag starts. |
| `OnDrag` | `MapDragEventArgs` | Continuous during drag. |
| `OnDragEnd` | `MapDragEventArgs` | Drag ends. |

## EventArgs Classes

**`MapPointerEventArgs`** -- pointer/interaction events:

| Property | Type | Description |
|----------|------|-------------|
| `ViewportX` | `double` | X coordinate in pixels. |
| `ViewportY` | `double` | Y coordinate in pixels. |
| `Position` | `LatLngLiteral?` | Geographic position. |
| `Button` | `int` | Mouse button (0=left, 1=middle, 2=right). |
| `PointerType` | `string?` | `"mouse"`, `"touch"`, or `"pen"`. |

**`MapDragEventArgs`** -- drag events:

| Property | Type | Description |
|----------|------|-------------|
| `ViewportX` | `double` | X coordinate in pixels. |
| `ViewportY` | `double` | Y coordinate in pixels. |
| `Position` | `LatLngLiteral?` | Geographic position of the drag point. |

**`MapViewChangeEventArgs`** -- view change events:

| Property | Type | Description |
|----------|------|-------------|
| `Center` | `LatLngLiteral?` | Current map center. |
| `Zoom` | `double` | Current zoom level. |
| `Tilt` | `double` | Current tilt angle in degrees. |
| `Heading` | `double` | Current heading in degrees. |

**`MapResizeEventArgs`** -- container resize:

| Property | Type | Description |
|----------|------|-------------|
| `Width` | `double` | New width in pixels. |
| `Height` | `double` | New height in pixels. |
