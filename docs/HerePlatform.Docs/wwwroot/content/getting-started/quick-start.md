---
title: Quick Start
category: Getting Started
order: 2
description: Display a HERE map in your Blazor app in under five minutes.
---

## Prerequisites

You need a **HERE API key**. Sign up at the [HERE Developer Portal](https://developer.here.com/) to get a free key.

## Register Services

In your `Program.cs`, call `AddBlazorHerePlatform` with your API key:

```csharp
using HerePlatformComponents;

builder.Services.AddBlazorHerePlatform("YOUR_API_KEY");
```

Never hard-code your key in source. Use configuration or user secrets instead:

```csharp
var apiKey = builder.Configuration["HerePlatform:ApiKey"]
    ?? throw new InvalidOperationException("HERE API key not configured.");

builder.Services.AddBlazorHerePlatform(apiKey);
```

## Add the Map Component

In any `.razor` page or component, add a `HereMap`:

```csharp
@using HerePlatformComponents
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Coordinates

<HereMap @ref="_map" Options="_options" Height="600px" />

@code {
    private HereMap _map = default!;

    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405), // Berlin
        Zoom = 12,
        LayerType = MapLayerType.VectorNormalMap
    };
}
```

This renders an interactive vector map centered on Berlin at zoom level 12. The default height is `500px`; the example above overrides it to `600px`.

## Run After Init

Use the `OnAfterInit` callback to run code once the map is ready:

```csharp
<HereMap @ref="_map" Options="_options" OnAfterInit="OnMapReady" />

@code {
    private HereMap _map = default!;

    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(48.8566, 2.3522), // Paris
        Zoom = 14
    };

    private async Task OnMapReady()
    {
        // The map's InteropObject is now available.
        var map = _map.InteropObject!;
        await map.AddMarkerAsync(new MarkerOptions
        {
            Position = new LatLngLiteral(48.8584, 2.2945) // Eiffel Tower
        });
    }
}
```

## Add Child Components

Map entities like markers, polylines, and polygons can also be declared as child components of `AdvancedHereMap`. For details, see the Components section.

## Satellite View

Switch to a satellite base layer by changing `LayerType`:

```csharp
private readonly MapOptions _options = new()
{
    Center = new LatLngLiteral(40.6892, -74.0445),
    Zoom = 16,
    LayerType = MapLayerType.RasterSatelliteMap
};
```

## Next Steps

- **Configuration** -- learn about `HereApiLoadOptions`, custom key services, and all `MapOptions` properties.
- **Blazor Server Notes** -- important differences when running server-side.
- **Components** -- markers, polylines, polygons, clustering, and more.
