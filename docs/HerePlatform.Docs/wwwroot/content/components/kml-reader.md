---
title: KmlReaderComponent
category: Components
order: 15
description: "Load and display KML data on the map."
apiRef: KmlReaderComponent
---

## Basic Usage

`KmlReaderComponent` loads KML (Keyhole Markup Language) data and renders it on the map. Like `GeoJsonReaderComponent`, it requires the data module:

```csharp
builder.Services.AddBlazorHerePlatform("YOUR_KEY", opts =>
{
    opts.LoadData = true;
});
```

Place it inside an `AdvancedHereMap` and provide a `Url`:

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Data

<AdvancedHereMap Options="_options" Height="500px">
    <KmlReaderComponent Url="data/routes.kml"
                        OnLoaded="HandleLoaded" />
</AdvancedHereMap>

@code {
    private readonly MapOptions _options = new()
    {
        Center = new LatLngLiteral(52.52, 13.405),
        Zoom = 10
    };

    private void HandleLoaded(GeoJsonLoadedEventArgs args)
    {
        Console.WriteLine($"Loaded {args.ObjectCount} KML objects");
    }
}
```

## URL Loading

Load KML from a local static file or a remote URL. Use `DefaultStyle` to override the KML's embedded styles, and `Visible` to toggle the layer.

```csharp
<KmlReaderComponent Url="https://example.com/data/regions.kml"
                    DefaultStyle="_style"
                    Visible="@_showKml"
                    OnLoaded="HandleLoaded" />

<button @onclick="() => _showKml = !_showKml">Toggle KML</button>

@code {
    private bool _showKml = true;

    private StyleOptions _style = new()
    {
        StrokeColor = "#FF6600",
        FillColor = "rgba(255, 102, 0, 0.2)",
        LineWidth = 3
    };

    private void HandleLoaded(GeoJsonLoadedEventArgs args)
    {
        Console.WriteLine($"Loaded {args.ObjectCount} objects from KML");
    }
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Url` | `string?` | `null` | URL of the KML file to load. |
| `DefaultStyle` | `StyleOptions?` | `null` | Default style applied to all loaded features. |
| `Visible` | `bool` | `true` | Controls layer visibility. |
| `OnLoaded` | `EventCallback<GeoJsonLoadedEventArgs>` | -- | Fires when KML data has been parsed. |

The `OnLoaded` callback uses `GeoJsonLoadedEventArgs`, which provides an `ObjectCount` property indicating how many map objects were created from the KML data.
