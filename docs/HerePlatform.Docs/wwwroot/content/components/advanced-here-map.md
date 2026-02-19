---
title: AdvancedHereMap
category: Components
order: 2
description: "Full-featured declarative map with child components and two-way binding."
apiRef: AdvancedHereMap
demo: map-controls
---

## Basic Usage

`AdvancedHereMap` wraps `HereMap` and adds declarative child component support, two-way binding for view properties, and a comprehensive event system.

```xml
<AdvancedHereMap Id="advanced-map"
                 Options="@options"
                 Height="500px"
                 OnAfterInit="@OnMapReady">
    <MarkerComponent Lat="52.52" Lng="13.405" />
</AdvancedHereMap>

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12
    };

    private Task OnMapReady() => Task.CompletedTask;
}
```

Child components are rendered inside a `CascadingValue` and are only mounted after the underlying map is initialized.

## Two-Way Binding

Bind map view properties directly to C# fields. Values update automatically when the user pans, zooms, tilts, or rotates the map.

```xml
<AdvancedHereMap Options="@options"
                 @bind-Center="center"
                 @bind-Zoom="zoom"
                 @bind-Tilt="tilt"
                 @bind-Heading="heading">
</AdvancedHereMap>

<p>Center: @center?.Lat, @center?.Lng</p>
<p>Zoom: @zoom | Tilt: @tilt | Heading: @heading</p>

@code {
    private MapOptions options = new() { Zoom = 10 };
    private LatLngLiteral? center = new(48.8566, 2.3522);
    private double? zoom = 10;
    private double? tilt = 0;
    private double? heading = 0;
}
```

Setting a bound property programmatically pushes the change to the JS map. Set `Animate="true"` to animate programmatic center and zoom changes.

## Layer Types

Configure the base layer via `MapOptions.LayerType`. Common options:

| LayerType | Description |
|-----------|-------------|
| `VectorNormalMap` | Default vector map (day) |
| `VectorNormalMapNight` | Dark-themed vector map |
| `RasterSatelliteMap` | Satellite imagery |
| `RasterTerrainMap` | Terrain map |
| `HybridDayRaster` | Satellite + road overlay |

Add or remove overlay layers at runtime:

```csharp
await mapRef.AddLayerAsync("vector.normal.traffic");
await mapRef.RemoveLayerAsync("vector.normal.traffic");
```

## Child Components

Nest any of these components inside `AdvancedHereMap`:

- `MarkerComponent` -- standard markers
- `DomMarkerComponent` -- DOM-based markers with custom HTML
- `PolylineComponent` -- line shapes
- `PolygonComponent` -- filled area shapes
- `CircleComponent` -- circles with center and radius
- `RectComponent` -- rectangles defined by geographic bounds
- `InfoBubbleComponent` -- popup bubbles
- `GroupComponent` -- logical grouping of map objects
- `MarkerClusterComponent` -- marker clustering

## Events

`AdvancedHereMap` exposes pointer, drag, view change, and collection events:

```xml
<AdvancedHereMap Options="@options"
                 OnClick="@HandleClick"
                 OnMapViewChangeEnd="@HandleViewChanged"
                 OnMarkersChanged="@HandleMarkersChanged">
</AdvancedHereMap>

@code {
    private void HandleClick(MapPointerEventArgs e)
    {
        Console.WriteLine($"Clicked at {e.Position?.Lat}, {e.Position?.Lng}");
    }

    private void HandleViewChanged(MapViewChangeEventArgs e)
    {
        Console.WriteLine($"Zoom: {e.Zoom}");
    }

    private void HandleMarkersChanged() { }
}
```

Key event groups:

- **Pointer**: `OnClick`, `OnDoubleClick`, `OnLongPress`, `OnContextMenu`, `OnPointerDown`, `OnPointerUp`, `OnPointerMove`, `OnPointerEnter`, `OnPointerLeave`
- **Drag**: `OnDragStart`, `OnDrag`, `OnDragEnd`
- **View**: `OnMapViewChange`, `OnMapViewChangeStart`, `OnMapViewChangeEnd`
- **Other**: `OnBaseLayerChange`, `OnEngineStateChange`, `OnResize`, `OnWheel`
- **Collections**: `OnMarkersChanged`, `OnPolygonsChanged`, `OnPolylinesChanged`, `OnCirclesChanged`, `OnRectsChanged`, `OnDomMarkersChanged`
