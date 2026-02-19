---
title: CircleComponent
category: Components
order: 7
description: "Circle shape with configurable center and radius."
apiRef: CircleComponent
---

## Basic Usage

`CircleComponent` draws a circle on the map defined by a center point and a radius in meters. Place it inside an `AdvancedHereMap`.

```xml
<AdvancedHereMap Options="@options">
    <CircleComponent CenterLat="52.52" CenterLng="13.405"
                     Radius="5000"
                     FillColor="rgba(52, 152, 219, 0.2)"
                     StrokeColor="#3498db"
                     LineWidth="2" />
</AdvancedHereMap>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 11
    };
}
```

Parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `CenterLat` | `double` | `0` | Center latitude. Two-way bindable via `@bind-CenterLat`. |
| `CenterLng` | `double` | `0` | Center longitude. Two-way bindable via `@bind-CenterLng`. |
| `Radius` | `double` | `0` | Radius in meters. Two-way bindable via `@bind-Radius`. |
| `FillColor` | `string?` | `null` | Fill color in CSS format. |
| `StrokeColor` | `string?` | `null` | Stroke color in CSS format. |
| `LineWidth` | `double?` | `null` | Border width in pixels. |
| `Precision` | `int?` | `null` | Number of points approximating the circle (default 72). |
| `LineCap` | `string?` | `null` | Cap style: `"round"`, `"square"`, or `"butt"`. |
| `LineJoin` | `string?` | `null` | Join style: `"round"`, `"miter"`, or `"bevel"`. |
| `LineDash` | `double[]?` | `null` | Dash pattern array. |
| `ZIndex` | `int?` | `null` | Stacking order. |
| `Visible` | `bool` | `true` | Controls visibility. |
| `Clickable` | `bool` | `false` | Enables click events. |
| `Draggable` | `bool` | `false` | Allows dragging. |

## Two-Way Binding

Bind the center position to track changes when the circle is dragged.

```xml
<AdvancedHereMap Options="@options">
    <CircleComponent @bind-CenterLat="lat"
                     @bind-CenterLng="lng"
                     Radius="3000"
                     Draggable="true"
                     FillColor="rgba(155, 89, 182, 0.3)"
                     StrokeColor="#8e44ad"
                     LineWidth="2" />
</AdvancedHereMap>

<p>Center: @lat.ToString("F4"), @lng.ToString("F4")</p>

@code {
    private double lat = 48.8566;
    private double lng = 2.3522;
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(48.8566, 2.3522),
        Zoom = 12
    };
}
```

The `CenterLatChanged` and `CenterLngChanged` callbacks fire when a drag ends.

## Styling

Customize the circle appearance with stroke, fill, and dash options.

```xml
<CircleComponent CenterLat="40.7128" CenterLng="-74.0060"
                 Radius="2000"
                 FillColor="rgba(231, 76, 60, 0.15)"
                 StrokeColor="#e74c3c"
                 LineWidth="3"
                 LineDash="new double[] { 10, 5 }"
                 Precision="90" />
```

Lower `Precision` values produce a visibly faceted shape, while higher values create smoother circles at the cost of more geometry points.

For clickable and draggable circles with full event support:

```xml
<CircleComponent CenterLat="52.52" CenterLng="13.405"
                 Radius="1000"
                 Clickable="true"
                 Draggable="true"
                 OnClick="@HandleClick"
                 OnDragEnd="@HandleDragEnd"
                 FillColor="rgba(46, 204, 113, 0.3)"
                 StrokeColor="#27ae60" />

@code {
    private void HandleClick(MapPointerEventArgs e)
    {
        Console.WriteLine("Circle clicked");
    }

    private void HandleDragEnd(MapDragEventArgs e)
    {
        Console.WriteLine("Circle drag ended");
    }
}
```

Available events: `OnClick`, `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, `OnPointerDown`, `OnPointerUp`, `OnPointerMove`, `OnPointerEnter`, `OnPointerLeave`, `OnDragStart`, `OnDrag`, `OnDragEnd`.
