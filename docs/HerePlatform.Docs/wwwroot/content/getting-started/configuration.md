---
title: Configuration
category: Getting Started
order: 3
description: Configure API loading options, map appearance, and API key management.
---

## AddBlazorHerePlatform Overloads

Three overloads are available for service registration:

**String API key** -- simplest form, uses default load options:

```csharp
builder.Services.AddBlazorHerePlatform("YOUR_API_KEY");
```

**HereApiLoadOptions** -- full control over which HERE JS modules are loaded:

```csharp
builder.Services.AddBlazorHerePlatform(new HereApiLoadOptions("YOUR_API_KEY")
{
    Language = "de",
    LoadClustering = true,
    LoadData = true
});
```

**Custom IBlazorHerePlatformKeyService** -- for dynamic key resolution or multi-tenant scenarios:

```csharp
builder.Services.AddBlazorHerePlatform(myCustomKeyService);
```

## HereApiLoadOptions

Controls how the HERE Maps JS API is loaded on the page.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ApiKey` | `string` | (required) | Your HERE API key |
| `Version` | `string` | `"3.1"` | HERE JS API version |
| `BaseUrl` | `string?` | `null` | Custom CDN base URL |
| `LoadMapEvents` | `bool` | `true` | Load the mapevents module |
| `LoadUI` | `bool` | `true` | Load the default UI controls |
| `LoadClustering` | `bool` | `false` | Load the clustering module |
| `LoadData` | `bool` | `false` | Load the data module (GeoJSON, KML) |
| `UseHarpEngine` | `bool` | `true` | Use the HARP rendering engine |
| `Language` | `string?` | `null` | Map label language (e.g. `"de"`, `"fr"`) |

Set `LoadClustering = true` if you use `MarkerClusterComponent`. Set `LoadData = true` if you use `GeoJsonReaderComponent` or `KmlReaderComponent`.

## MapOptions

Passed to the `HereMap` component via the `Options` parameter.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Center` | `LatLngLiteral?` | `null` | Initial map center (lat, lng) |
| `Zoom` | `double` | `10` | Initial zoom level |
| `LayerType` | `MapLayerType` | `VectorNormalMap` | Base layer type |
| `EnableInteraction` | `bool` | `true` | Enable drag, pinch, scroll |
| `EnableUI` | `bool` | `true` | Show default UI controls |
| `MinZoom` | `double?` | `null` | Minimum allowed zoom |
| `MaxZoom` | `double?` | `null` | Maximum allowed zoom |
| `Tilt` | `double?` | `null` | 3D tilt angle in degrees |
| `Heading` | `double?` | `null` | Map rotation in degrees |
| `Padding` | `Padding?` | `null` | Viewport padding |
| `AutoColor` | `bool?` | `null` | Auto UI color adaptation |
| `UiLocale` | `string?` | `null` | UI control locale (e.g. `"de-DE"`) |
| `PixelRatio` | `double?` | `null` | Pixel ratio for high-DPI displays |
| `FixedCenter` | `bool?` | `null` | Keep center fixed during resize |

Example with tilt and heading for a 3D perspective:

```csharp
var options = new MapOptions
{
    Center = new LatLngLiteral(52.52, 13.405),
    Zoom = 16,
    Tilt = 60,
    Heading = 45,
    LayerType = MapLayerType.VectorNormalMap
};
```

## API Key Management

The `IBlazorHerePlatformKeyService` interface manages the API key lifecycle:

```csharp
public interface IBlazorHerePlatformKeyService
{
    Task<HereApiLoadOptions> GetApiOptions();
    bool IsApiInitialized { get; }
    void MarkApiInitialized();
    void UpdateApiKey(string apiKey);
}
```

The built-in `BlazorHerePlatformKeyService` stores the key in memory. Implement `IBlazorHerePlatformKeyService` yourself to load keys from a database, vault, or per-user configuration. Call `UpdateApiKey` to swap the key at runtime; this resets `IsApiInitialized`, so the next map render reloads the HERE JS API with the new key.

## Per-Map Load Options Override

You can also attach `HereApiLoadOptions` directly to a specific map's `MapOptions`:

```csharp
var options = new MapOptions
{
    Center = new LatLngLiteral(52.52, 13.405),
    Zoom = 12,
    ApiLoadOptions = new HereApiLoadOptions("DIFFERENT_KEY")
    {
        LoadClustering = true
    }
};
```

When `ApiLoadOptions` is set on a `MapOptions` instance, it takes priority over the DI-registered key service.
