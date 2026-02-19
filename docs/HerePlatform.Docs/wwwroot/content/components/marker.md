---
title: MarkerComponent
category: Components
order: 3
description: "Standard HERE marker with drag, click, and InfoBubble support."
apiRef: MarkerComponent
demo: map-markers
---

## Basic Usage

Place `MarkerComponent` inside an `AdvancedHereMap` to add a marker at the specified coordinates.

```xml
<AdvancedHereMap Options="@options">
    <MarkerComponent Lat="52.52" Lng="13.405" />
</AdvancedHereMap>
```

Common parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Lat` | `double` | `0` | Latitude in degrees. Two-way bindable via `@bind-Lat`. |
| `Lng` | `double` | `0` | Longitude in degrees. Two-way bindable via `@bind-Lng`. |
| `Clickable` | `bool` | `false` | Enables tap/click events. |
| `Draggable` | `bool` | `false` | Allows the user to drag the marker. |
| `Title` | `string?` | `null` | Tooltip text for accessibility. |
| `Visible` | `bool` | `true` | Controls marker visibility. |
| `Opacity` | `double?` | `null` | Opacity from 0 to 1. |
| `ZIndex` | `int?` | `null` | Stacking order. |
| `MinZoom` | `double?` | `null` | Minimum zoom level for visibility. |
| `MaxZoom` | `double?` | `null` | Maximum zoom level for visibility. |
| `IconUrl` | `string?` | `null` | URL or inline SVG for a custom icon. |

## Dragging

Enable `Draggable` and use two-way binding to track the marker position as the user drags it.

```xml
<AdvancedHereMap Options="@options">
    <MarkerComponent @bind-Lat="lat" @bind-Lng="lng" Draggable="true" />
</AdvancedHereMap>

<p>Position: @lat, @lng</p>

@code {
    private double lat = 52.52;
    private double lng = 13.405;
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };
}
```

The `LatChanged` and `LngChanged` callbacks fire when the drag ends.

## Custom Icons

Set `IconUrl` to a URL pointing to an image or an inline SVG string.

```xml
<MarkerComponent Lat="48.8566" Lng="2.3522"
                 IconUrl="/images/pin-blue.png" />
```

Using inline SVG:

```csharp
private string svgIcon = """
    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24">
        <circle cx="12" cy="12" r="10" fill="red" stroke="white" stroke-width="2"/>
    </svg>
    """;
```

```xml
<MarkerComponent Lat="48.8566" Lng="2.3522" IconUrl="@svgIcon" />
```

## InfoBubble via ChildContent

Provide `ChildContent` to show an InfoBubble when the marker is tapped. The content is rendered as static HTML inside the bubble.

```xml
<MarkerComponent Lat="52.52" Lng="13.405" Clickable="true">
    <h3>Berlin</h3>
    <p>Capital of Germany</p>
</MarkerComponent>
```

## Events

All map object events from `MapObjectComponentBase` are available:

```xml
<MarkerComponent Lat="52.52" Lng="13.405"
                 Clickable="true"
                 OnClick="@HandleTap"
                 OnDragEnd="@HandleDragEnd" />

@code {
    private void HandleTap(MapPointerEventArgs e)
    {
        Console.WriteLine($"Marker tapped at {e.Position?.Lat}, {e.Position?.Lng}");
    }

    private void HandleDragEnd(MapDragEventArgs e)
    {
        Console.WriteLine($"Dragged to {e.Position?.Lat}, {e.Position?.Lng}");
    }
}
```

Available events: `OnClick`, `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, `OnPointerDown`, `OnPointerUp`, `OnPointerMove`, `OnPointerEnter`, `OnPointerLeave`, `OnDragStart`, `OnDrag`, `OnDragEnd`.
