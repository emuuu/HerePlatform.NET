---
title: PolygonComponent
category: Components
order: 6
description: "Polygon shape for drawing filled areas."
apiRef: PolygonComponent
---

## Basic Usage

`PolygonComponent` draws a filled polygon on the map. Place it inside an `AdvancedHereMap` and provide a list of coordinates defining the boundary.

```xml
<AdvancedHereMap Options="@options">
    <PolygonComponent Path="@path"
                      FillColor="rgba(52, 152, 219, 0.3)"
                      StrokeColor="#2980b9"
                      LineWidth="2" />
</AdvancedHereMap>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 11
    };

    private List<LatLngLiteral> path = new()
    {
        new(52.54, 13.35),
        new(52.54, 13.45),
        new(52.50, 13.45),
        new(52.50, 13.35)
    };
}
```

Parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | `List<LatLngLiteral>?` | `null` | Boundary coordinates. Two-way bindable via `@bind-Path`. |
| `FillColor` | `string?` | `null` | Fill color in CSS format. |
| `StrokeColor` | `string?` | `null` | Stroke color in CSS format. |
| `LineWidth` | `double?` | `null` | Border line width in pixels. |
| `LineCap` | `string?` | `null` | Cap style: `"round"`, `"square"`, or `"butt"`. |
| `LineJoin` | `string?` | `null` | Join style: `"round"`, `"miter"`, or `"bevel"`. |
| `LineDash` | `double[]?` | `null` | Dash pattern array. |
| `Holes` | `List<List<LatLngLiteral>>?` | `null` | Interior rings (holes) cut out of the polygon. |
| `ZIndex` | `int?` | `null` | Stacking order. |
| `Visible` | `bool` | `true` | Controls visibility. |
| `Clickable` | `bool` | `false` | Enables click events. |
| `Draggable` | `bool` | `false` | Allows dragging. |
| `Extrusion` | `double?` | `null` | 3D extrusion height in meters (HARP engine). |

## Styling

Create polygons with holes to represent complex areas like parks with lakes:

```xml
<PolygonComponent Path="@outerRing"
                  Holes="@holes"
                  FillColor="rgba(46, 204, 113, 0.4)"
                  StrokeColor="#27ae60"
                  LineWidth="2"
                  LineDash="new double[] { 6, 3 }" />

@code {
    private List<LatLngLiteral> outerRing = new()
    {
        new(52.54, 13.35), new(52.54, 13.45),
        new(52.50, 13.45), new(52.50, 13.35)
    };

    private List<List<LatLngLiteral>> holes = new()
    {
        new()
        {
            new(52.53, 13.38), new(52.53, 13.42),
            new(52.51, 13.42), new(52.51, 13.38)
        }
    };
}
```

For 3D extruded buildings (requires HARP engine):

```xml
<PolygonComponent Path="@buildingFootprint"
                  FillColor="rgba(149, 165, 166, 0.8)"
                  Extrusion="50"
                  Elevation="0" />
```

## Events

Enable `Clickable` or `Draggable` to receive events. Use `@bind-Path` to track geometry updates after dragging.

```xml
<PolygonComponent @bind-Path="path"
                  FillColor="rgba(231, 76, 60, 0.3)"
                  StrokeColor="#c0392b"
                  LineWidth="2"
                  Clickable="true"
                  Draggable="true"
                  OnClick="@HandleClick"
                  OnDragEnd="@HandleDragEnd" />

@code {
    private void HandleClick(MapPointerEventArgs e)
    {
        Console.WriteLine($"Polygon clicked at {e.Position?.Lat}, {e.Position?.Lng}");
    }

    private void HandleDragEnd(MapDragEventArgs e)
    {
        Console.WriteLine("Polygon was dragged to a new position.");
    }
}
```

Available events: `OnClick`, `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, `OnPointerDown`, `OnPointerUp`, `OnPointerMove`, `OnPointerEnter`, `OnPointerLeave`, `OnDragStart`, `OnDrag`, `OnDragEnd`.
