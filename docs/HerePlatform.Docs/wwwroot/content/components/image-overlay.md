---
title: ImageOverlayComponent
category: Components
order: 17
description: "Overlay a static image on the map."
apiRef: ImageOverlayComponent
---

## Basic Usage

`ImageOverlayComponent` stretches a static image (PNG, JPEG, or SVG) over a geographic bounding box on the map. Place it inside an `AdvancedHereMap`.

```csharp
@using HerePlatformComponents.Maps

<AdvancedHereMap Options="_options" Height="500px">
    <ImageOverlayComponent ImageUrl="images/floor-plan.png"
                           Top="52.525"
                           Left="13.40"
                           Bottom="52.515"
                           Right="13.42"
                           Opacity="0.75" />
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.41),
        Zoom = 15
    };
}
```

The image is projected onto the map and scales with zoom. This is useful for floor plans, historical maps, or custom overlays tied to specific geographic coordinates.

## Bounds Configuration

The overlay bounds are defined by four parameters that form a geographic rectangle:

```
Top (north latitude)    +-----------+
                        |           |
                        |   image   |
                        |           |
Bottom (south latitude) +-----------+
                   Left            Right
              (west longitude)  (east longitude)
```

```csharp
<ImageOverlayComponent ImageUrl="@_imageUrl"
                       Top="48.86"
                       Left="2.29"
                       Bottom="48.85"
                       Right="2.30"
                       Opacity="@_opacity"
                       Visible="@_showOverlay" />

<input type="range" min="0" max="1" step="0.05"
       @bind="_opacity" @bind:event="oninput" />

<button @onclick="() => _showOverlay = !_showOverlay">
    Toggle Overlay
</button>

@code {
    private string _imageUrl = "images/historic-map.png";
    private double _opacity = 0.8;
    private bool _showOverlay = true;
}
```

Both HTTP URLs and data URIs are supported for `ImageUrl`. For example, you can use a base64-encoded image:

```csharp
private string _imageUrl = "data:image/png;base64,iVBORw0KGgo...";
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ImageUrl` | `string?` | `null` | URL of the image (HTTP URL or data URI). |
| `Top` | `double` | `0` | North latitude of the bounding box. |
| `Left` | `double` | `0` | West longitude of the bounding box. |
| `Bottom` | `double` | `0` | South latitude of the bounding box. |
| `Right` | `double` | `0` | East longitude of the bounding box. |
| `Opacity` | `double` | `1.0` | Overlay opacity from 0 (transparent) to 1 (opaque). |
| `Visible` | `bool` | `true` | Controls overlay visibility. |
