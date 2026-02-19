---
title: PolylineComponent
category: Components
order: 5
description: "Polyline shape for drawing lines on the map."
apiRef: PolylineComponent
---

## Basic Usage

`PolylineComponent` draws a line through a series of geographic coordinates. Place it inside an `AdvancedHereMap`.

```xml
<AdvancedHereMap Options="@options">
    <PolylineComponent Path="@path" StrokeColor="rgba(0, 128, 255, 0.7)" LineWidth="4" />
</AdvancedHereMap>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 10
    };

    private List<LatLngLiteral> path = new()
    {
        new(52.52, 13.405),
        new(52.50, 13.45),
        new(52.49, 13.38)
    };
}
```

Parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | `List<LatLngLiteral>?` | `null` | Coordinates defining the polyline. Two-way bindable via `@bind-Path`. |
| `StrokeColor` | `string?` | `null` | Stroke color in CSS format. |
| `LineWidth` | `double?` | `null` | Line width in pixels. |
| `LineCap` | `string?` | `null` | Cap style: `"round"`, `"square"`, or `"butt"`. |
| `LineJoin` | `string?` | `null` | Join style: `"round"`, `"miter"`, or `"bevel"`. |
| `LineDash` | `double[]?` | `null` | Dash pattern array `[dash, gap, ...]`. |
| `Arrows` | `bool?` | `null` | When true, renders directional arrows on the line. |
| `ZIndex` | `int?` | `null` | Stacking order. |
| `Visible` | `bool` | `true` | Controls visibility. |
| `Draggable` | `bool` | `false` | Allows dragging the polyline. |
| `Clickable` | `bool` | `false` | Enables click events. |
| `Extrusion` | `double?` | `null` | 3D extrusion height in meters (HARP engine). |
| `Elevation` | `double?` | `null` | 3D base elevation in meters (HARP engine). |

## Styling

Customize the appearance with stroke options and dash patterns.

```xml
<PolylineComponent Path="@path"
                   StrokeColor="#e74c3c"
                   LineWidth="3"
                   LineDash="new double[] { 8, 4 }"
                   LineCap="round"
                   Arrows="true" />
```

For a dashed line with directional arrows:

```csharp
private double[] dashPattern = [10, 6, 2, 6];
```

```xml
<PolylineComponent Path="@path"
                   StrokeColor="rgba(44, 62, 80, 0.8)"
                   LineWidth="2"
                   LineDash="@dashPattern"
                   Arrows="true" />
```

## Interactive Polylines

Enable `Clickable` for events or `Draggable` for drag support. Use `@bind-Path` to track geometry changes after dragging.

```xml
<PolylineComponent @bind-Path="path"
                   StrokeColor="#2ecc71"
                   LineWidth="5"
                   Clickable="true"
                   Draggable="true"
                   OnClick="@HandleClick"
                   OnDragEnd="@HandleDragEnd" />

@code {
    private List<LatLngLiteral> path = new()
    {
        new(52.52, 13.405),
        new(52.50, 13.45)
    };

    private void HandleClick(MapPointerEventArgs e)
    {
        Console.WriteLine("Polyline clicked");
    }

    private void HandleDragEnd(MapDragEventArgs e)
    {
        Console.WriteLine($"Polyline dragged. New path has {path.Count} points.");
    }
}
```

Available events: `OnClick`, `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, `OnPointerDown`, `OnPointerUp`, `OnPointerMove`, `OnPointerEnter`, `OnPointerLeave`, `OnDragStart`, `OnDrag`, `OnDragEnd`.
