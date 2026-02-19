---
title: Map Image Service
category: Services
order: 14
description: "Generate static map images."
apiRef: IMapImageService
---

## Setup

Inject `IMapImageService` into your component. It is registered automatically by `AddBlazorHerePlatform`.

```csharp
@inject IMapImageService MapImageService
```

## GetImageAsync

Generate a static map image centered on a location. Returns raw image bytes that can be displayed directly or saved to disk.

```csharp
var imageBytes = await MapImageService.GetImageAsync(new MapImageRequest
{
    Center = new LatLngLiteral(52.5200, 13.4050),
    Zoom = 14,
    Width = 800,
    Height = 600,
    Format = MapImageFormat.Png,
    Style = MapImageStyle.Default
});

// Display in Blazor as a data URI
var base64 = Convert.ToBase64String(imageBytes);
var dataUri = $"data:image/png;base64,{base64}";
```

```html
<img src="@dataUri" alt="Map" />
```

## MapImageRequest Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Center` | `LatLngLiteral` | -- | Map center (required). |
| `Zoom` | `int` | `12` | Zoom level (0–22). |
| `Width` | `int` | `512` | Image width in pixels. |
| `Height` | `int` | `512` | Image height in pixels. |
| `Format` | `MapImageFormat` | `Png` | Output format. |
| `Style` | `MapImageStyle` | `Default` | Map style/theme. |
| `Ppi` | `int` | `72` | Pixels per inch (72, 150, 250, 320, 400, 500). |

## MapImageFormat Enum

| Value | Description |
|-------|-------------|
| `Png` | PNG format (lossless). |
| `Jpeg` | JPEG format (smaller file size). |

## MapImageStyle Enum

| Value | Wire Value | Description |
|-------|------------|-------------|
| `Default` | `explore.day` | Standard day map. |
| `Night` | `explore.night` | Dark theme. |
| `Satellite` | `explore.satellite.day` | Satellite imagery. |
| `Lite` | `lite.day` | Simplified day map. |
| `LiteNight` | `lite.night` | Simplified dark map. |

## Displaying Images

The service returns `byte[]` directly. Convert to a Base64 data URI for inline display, or stream to a file:

```csharp
// Save to file
await File.WriteAllBytesAsync("map.png", imageBytes);
```
