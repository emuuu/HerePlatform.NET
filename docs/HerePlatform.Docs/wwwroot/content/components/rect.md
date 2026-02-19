---
title: RectComponent
category: Components
order: 8
description: "Rectangle shape defined by geographic bounds."
apiRef: RectComponent
---

## Basic Usage

`RectComponent` draws a rectangle on the map defined by four geographic bounds: `Top` (north latitude), `Left` (west longitude), `Bottom` (south latitude), and `Right` (east longitude). Place it inside an `AdvancedHereMap`.

```xml
<AdvancedHereMap Options="@options">
    <RectComponent Top="52.54" Left="13.35"
                   Bottom="52.50" Right="13.45"
                   FillColor="rgba(52, 152, 219, 0.2)"
                   StrokeColor="#2980b9"
                   LineWidth="2" />
</AdvancedHereMap>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.40),
        Zoom = 12
    };
}
```

Parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Top` | `double` | `0` | North latitude. Two-way bindable via `@bind-Top`. |
| `Left` | `double` | `0` | West longitude. Two-way bindable via `@bind-Left`. |
| `Bottom` | `double` | `0` | South latitude. Two-way bindable via `@bind-Bottom`. |
| `Right` | `double` | `0` | East longitude. Two-way bindable via `@bind-Right`. |
| `FillColor` | `string?` | `null` | Fill color in CSS format. |
| `StrokeColor` | `string?` | `null` | Stroke color in CSS format. |
| `LineWidth` | `double?` | `null` | Border width in pixels. |
| `LineCap` | `string?` | `null` | Cap style: `"round"`, `"square"`, or `"butt"`. |
| `LineJoin` | `string?` | `null` | Join style: `"round"`, `"miter"`, or `"bevel"`. |
| `LineDash` | `double[]?` | `null` | Dash pattern array. |
| `ZIndex` | `int?` | `null` | Stacking order. |
| `Visible` | `bool` | `true` | Controls visibility. |
| `Clickable` | `bool` | `false` | Enables click events. |
| `Draggable` | `bool` | `false` | Allows dragging. |
| `Extrusion` | `double?` | `null` | 3D extrusion height in meters (HARP engine). |
| `Elevation` | `double?` | `null` | 3D base elevation in meters (HARP engine). |
| `Data` | `object?` | `null` | Arbitrary data attached to the rectangle. |

Use two-way binding to track bounds changes when the rectangle is dragged:

```xml
<RectComponent @bind-Top="top" @bind-Left="left"
               @bind-Bottom="bottom" @bind-Right="right"
               Draggable="true"
               FillColor="rgba(155, 89, 182, 0.2)"
               StrokeColor="#8e44ad" />

@code {
    private double top = 52.54, left = 13.35;
    private double bottom = 52.50, right = 13.45;
}
```

## Styling

Customize the rectangle with stroke options, fill colors, and dash patterns.

```xml
<RectComponent Top="48.88" Left="2.32"
               Bottom="48.83" Right="2.38"
               FillColor="rgba(231, 76, 60, 0.15)"
               StrokeColor="#c0392b"
               LineWidth="3"
               LineDash="new double[] { 8, 4 }"
               LineJoin="round" />
```

For a selection highlight with no fill:

```xml
<RectComponent Top="@selTop" Left="@selLeft"
               Bottom="@selBottom" Right="@selRight"
               StrokeColor="#f39c12"
               LineWidth="2"
               LineDash="new double[] { 4, 4 }" />
```

Interactive rectangles with full event support:

```xml
<RectComponent Top="52.54" Left="13.35"
               Bottom="52.50" Right="13.45"
               Clickable="true"
               Draggable="true"
               OnClick="@HandleClick"
               OnDragEnd="@HandleDragEnd"
               FillColor="rgba(46, 204, 113, 0.2)"
               StrokeColor="#27ae60"
               LineWidth="2" />

@code {
    private void HandleClick(MapPointerEventArgs e)
    {
        Console.WriteLine("Rectangle clicked");
    }

    private void HandleDragEnd(MapDragEventArgs e)
    {
        Console.WriteLine("Rectangle dragged");
    }
}
```

Available events: `OnClick`, `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, `OnPointerDown`, `OnPointerUp`, `OnPointerMove`, `OnPointerEnter`, `OnPointerLeave`, `OnDragStart`, `OnDrag`, `OnDragEnd`.
