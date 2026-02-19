---
title: DomMarkerComponent
category: Components
order: 4
description: "DOM-based marker with custom HTML content."
apiRef: DomMarkerComponent
---

## Basic Usage

`DomMarkerComponent` renders a marker using a DOM element (`H.map.DomIcon`) instead of a canvas-drawn icon. Place it inside an `AdvancedHereMap`.

```xml
<AdvancedHereMap Options="@options">
    <DomMarkerComponent Lat="52.52" Lng="13.405">
        <div style="background: #1a73e8; color: white; padding: 4px 8px;
                    border-radius: 4px; font-size: 12px;">
            Berlin
        </div>
    </DomMarkerComponent>
</AdvancedHereMap>
```

Parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Lat` | `double` | `0` | Latitude. Two-way bindable via `@bind-Lat`. |
| `Lng` | `double` | `0` | Longitude. Two-way bindable via `@bind-Lng`. |
| `Clickable` | `bool` | `false` | Enables click events. |
| `Draggable` | `bool` | `false` | Allows dragging. |
| `Visible` | `bool` | `true` | Controls visibility. |
| `Opacity` | `double?` | `null` | Opacity (0-1). |
| `ZIndex` | `int?` | `null` | Stacking order. |
| `MinZoom` | `double?` | `null` | Minimum zoom for visibility. |
| `MaxZoom` | `double?` | `null` | Maximum zoom for visibility. |
| `Data` | `object?` | `null` | Arbitrary data attached to the marker. |

## Custom HTML

The `ChildContent` render fragment provides the HTML template for the `DomIcon`. The content is captured as static HTML -- Blazor interactivity (event handlers, `@bind`) is not available inside the DOM marker template.

```xml
<DomMarkerComponent Lat="48.8566" Lng="2.3522">
    <div class="custom-pin">
        <img src="/images/avatar.jpg" style="width:32px; height:32px; border-radius:50%;" />
        <span>User Location</span>
    </div>
</DomMarkerComponent>
```

For complex layouts, define styles in your CSS file and reference classes in the template:

```xml
<DomMarkerComponent Lat="40.7128" Lng="-74.0060">
    <div class="price-tag">$250/night</div>
</DomMarkerComponent>
```

`DomMarkerComponent` also works inside a `GroupComponent` for logical grouping and bulk visibility control.

## Dragging with Two-Way Binding

```xml
<DomMarkerComponent @bind-Lat="lat" @bind-Lng="lng" Draggable="true">
    <div class="drag-pin">Drag me</div>
</DomMarkerComponent>

@code {
    private double lat = 52.52;
    private double lng = 13.405;
}
```

## Events

`DomMarkerComponent` inherits all events from `MapObjectComponentBase`:

```xml
<DomMarkerComponent Lat="52.52" Lng="13.405"
                    Clickable="true"
                    OnClick="@HandleClick"
                    OnDragEnd="@HandleDragEnd">
    <div class="label">Click me</div>
</DomMarkerComponent>

@code {
    private void HandleClick(MapPointerEventArgs e)
    {
        Console.WriteLine("DomMarker clicked");
    }

    private void HandleDragEnd(MapDragEventArgs e)
    {
        Console.WriteLine($"New position: {e.Position?.Lat}, {e.Position?.Lng}");
    }
}
```

Available events: `OnClick`, `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, `OnPointerDown`, `OnPointerUp`, `OnPointerMove`, `OnPointerEnter`, `OnPointerLeave`, `OnDragStart`, `OnDrag`, `OnDragEnd`.
