---
title: GeoJsonReaderComponent
category: Components
order: 14
description: "Load and display GeoJSON data on the map."
apiRef: GeoJsonReaderComponent
demo: map-data
---

## Basic Usage

`GeoJsonReaderComponent` loads GeoJSON data and renders the features on the map. It requires the data module to be enabled:

```csharp
builder.Services.AddBlazorHerePlatform("YOUR_KEY", opts =>
{
    opts.LoadData = true;
});
```

Place the component inside an `AdvancedHereMap`:

```csharp
@using HerePlatformComponents.Maps
@using HerePlatformComponents.Maps.Data

<AdvancedHereMap Options="_options" Height="500px">
    <GeoJsonReaderComponent Url="data/districts.geojson"
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
        Console.WriteLine($"Loaded {args.ObjectCount} objects");
    }
}
```

## Inline Data

Pass a GeoJSON string directly with the `GeoJsonString` parameter instead of loading from a URL:

```csharp
<GeoJsonReaderComponent GeoJsonString="@_geoJson" />

@code {
    private string _geoJson = """
        {
            "type": "FeatureCollection",
            "features": [
                {
                    "type": "Feature",
                    "geometry": {
                        "type": "Point",
                        "coordinates": [13.405, 52.52]
                    },
                    "properties": { "name": "Berlin" }
                }
            ]
        }
        """;
}
```

When both `Url` and `GeoJsonString` are set, `Url` takes precedence.

## URL Loading

Load GeoJSON from a remote endpoint or a local static file:

```csharp
<GeoJsonReaderComponent Url="https://example.com/api/boundaries.geojson"
                        Visible="@_showLayer"
                        OnLoaded="HandleLoaded" />
```

Toggle `Visible` to show or hide the loaded data without re-fetching.

## Styling

Apply a default style to all loaded features with the `DefaultStyle` parameter:

```csharp
<GeoJsonReaderComponent Url="data/parks.geojson"
                        DefaultStyle="_style" />

@code {
    private StyleOptions _style = new()
    {
        StrokeColor = "rgba(0, 128, 0, 0.8)",
        FillColor = "rgba(0, 200, 0, 0.3)",
        LineWidth = 2
    };
}
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Url` | `string?` | `null` | URL of the GeoJSON file to load. |
| `GeoJsonString` | `string?` | `null` | Inline GeoJSON string. |
| `DefaultStyle` | `StyleOptions?` | `null` | Default style applied to all loaded features. |
| `Visible` | `bool` | `true` | Controls layer visibility. |
| `OnLoaded` | `EventCallback<GeoJsonLoadedEventArgs>` | -- | Fires when GeoJSON data has been parsed. |
