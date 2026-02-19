---
title: CustomTileLayerComponent
category: Components
order: 16
description: "Custom tile layer for external tile providers."
apiRef: CustomTileLayerComponent
---

## Basic Usage

`CustomTileLayerComponent` overlays raster tiles from an external tile provider on top of the HERE base map. Place it inside an `AdvancedHereMap` and provide a URL pattern.

```csharp
@using HerePlatformComponents.Maps

<AdvancedHereMap Options="_options" Height="500px">
    <CustomTileLayerComponent UrlPattern="@_tileUrl"
                              Opacity="0.7" />
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 10
    };

    private string _tileUrl =
        "https://tile.openweathermap.org/map/temp_new/{z}/{x}/{y}.png?appid=YOUR_OWM_KEY";
}
```

## Tile URL Template

The `UrlPattern` string must include `{x}`, `{y}`, and `{z}` placeholders. These are replaced at runtime with the tile column, row, and zoom level values respectively.

Common tile provider examples:

```csharp
// OpenStreetMap
var osmUrl = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

// Stamen Toner
var stamenUrl = "https://tiles.stadiamaps.com/tiles/stamen_toner/{z}/{x}/{y}.png";

// Custom tile server
var customUrl = "https://tiles.example.com/overlay/{z}/{x}/{y}.png";
```

Configure the zoom range and tile size to match the provider's specifications:

```csharp
<CustomTileLayerComponent UrlPattern="@_tileUrl"
                          Min="2"
                          Max="18"
                          TileSize="256"
                          Opacity="0.5"
                          Visible="@_showOverlay" />

<button @onclick="() => _showOverlay = !_showOverlay">Toggle Overlay</button>

@code {
    private bool _showOverlay = true;
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `UrlPattern` | `string?` | `null` | URL with `{x}`, `{y}`, `{z}` placeholders. |
| `Min` | `int` | `0` | Minimum zoom level for the tile layer. |
| `Max` | `int` | `20` | Maximum zoom level for the tile layer. |
| `TileSize` | `int` | `256` | Tile size in pixels. |
| `Opacity` | `double` | `1.0` | Layer opacity from 0 (transparent) to 1 (opaque). |
| `Visible` | `bool` | `true` | Controls layer visibility. |

The custom tile layer renders on top of the base map. Adjust `Opacity` to blend with the underlying map, or set `Visible` to `false` to hide it without removing the component.
