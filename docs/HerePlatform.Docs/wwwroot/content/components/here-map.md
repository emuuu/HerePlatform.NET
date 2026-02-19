---
title: HereMap
category: Components
order: 1
description: "Minimal HERE map component."
apiRef: HereMap
demo: map-basic
---

## Basic Usage

`HereMap` renders a single HERE Maps container with minimal configuration. It creates the map on first render and manages its lifecycle.

```xml
<HereMap Id="my-map"
         Height="400px"
         Options="@options" />

@code {
    private MapOptions options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 12,
        LayerType = MapLayerType.VectorNormalMap
    };
}
```

The map suppresses Blazor re-renders (`ShouldRender` returns `false`) because visual state is managed entirely by the HERE Maps JS API. Use `ReInitializeAsync` to rebuild the map after option changes.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string?` | `null` | HTML `id` attribute for the map container. |
| `Options` | `MapOptions?` | `null` | Initial map configuration (center, zoom, layer type, UI options). |
| `Height` | `string` | `"500px"` | CSS height value applied as an inline style. |
| `CssClass` | `string?` | `null` | CSS class(es) applied to the map container div. |

`MapOptions` supports the following properties:

```csharp
var options = new MapOptions
{
    Center = new LatLngLiteral(48.8566, 2.3522),
    Zoom = 14,
    LayerType = MapLayerType.RasterSatelliteMap,
    EnableInteraction = true,
    EnableUI = true,
    MinZoom = 3,
    MaxZoom = 18,
    Tilt = 45,
    Heading = 90,
    UiLocale = "de-DE",
    PixelRatio = 2.0
};
```

## Events

### OnAfterInit

Fires after the map has been initialized and is ready for interaction.

```xml
<HereMap Options="@options" OnAfterInit="@HandleMapReady" />

@code {
    private async Task HandleMapReady()
    {
        Console.WriteLine("Map is ready.");
    }
}
```

## Methods

### ReInitializeAsync

Disposes the current map and re-creates it with the provided (or current) `Options`. Use this when changing options that require a full map rebuild, such as `UiLocale`.

```csharp
@code {
    private HereMap mapRef = default!;

    private async Task ChangeLocale()
    {
        var newOptions = new MapOptions
        {
            Center = new LatLngLiteral(48.8566, 2.3522),
            Zoom = 12,
            UiLocale = "fr-FR"
        };
        await mapRef.ReInitializeAsync(newOptions);
    }
}
```

```xml
<HereMap @ref="mapRef" Options="@options" />
<button @onclick="ChangeLocale">Switch to French</button>
```
