---
title: Geocoding Service
category: Services
order: 2
description: "Forward and reverse geocoding."
apiRef: IGeocodingService
demo: map-geocoding
---

## Forward Geocoding (GeocodeAsync)

Convert an address or place name into geographic coordinates.

```csharp
@inject IGeocodingService GeocodingService
```

```csharp
var result = await GeocodingService.GeocodeAsync("Brandenburger Tor, Berlin");

foreach (var item in result.Items ?? [])
{
    Console.WriteLine($"{item.Title}: {item.Position?.Lat}, {item.Position?.Lng}");
    Console.WriteLine($"Address: {item.Address}");
    Console.WriteLine($"Type: {item.ResultType}");
}
```

Each `GeocodeItem` contains:

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string?` | Display name of the result. |
| `Position` | `LatLngLiteral?` | Geographic coordinates. |
| `Address` | `string?` | Formatted address string. |
| `ResultType` | `string?` | Type such as `"houseNumber"`, `"street"`, `"locality"`. |

## Reverse Geocoding (ReverseGeocodeAsync)

Convert coordinates into an address.

```csharp
var position = new LatLngLiteral(52.5163, 13.3777);
var result = await GeocodingService.ReverseGeocodeAsync(position);

var address = result.Items?.FirstOrDefault();
if (address is not null)
{
    Console.WriteLine($"Address: {address.Address}");
}
```

## Options

Both methods accept an optional `GeocodeOptions` parameter:

```csharp
var options = new GeocodeOptions
{
    Lang = "de",           // Language for results
    Limit = 3,             // Max results (default: 5)
    BoundingBox = "52.3,13.1,52.7,13.6" // "south,west,north,east"
};

var result = await GeocodingService.GeocodeAsync("Alexanderplatz", options);
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Lang` | `string?` | `null` | Language code (e.g. `"en"`, `"de"`). |
| `Limit` | `int` | `5` | Maximum number of results. |
| `BoundingBox` | `string?` | `null` | Bounding box to narrow results. Format: `"south,west,north,east"`. |

Reverse geocoding with options:

```csharp
var options = new GeocodeOptions { Lang = "en", Limit = 1 };
var result = await GeocodingService.ReverseGeocodeAsync(
    new LatLngLiteral(48.8566, 2.3522), options);
```
