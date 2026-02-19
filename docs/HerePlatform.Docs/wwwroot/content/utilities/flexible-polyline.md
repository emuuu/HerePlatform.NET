---
title: Flexible Polyline
category: Utilities
order: 3
description: "Encode and decode HERE flexible polyline format."
apiRef: FlexiblePolyline
---

## Encoding

`FlexiblePolyline` is a static utility class that implements the HERE Flexible Polyline codec in pure C#. Use `Encode` to compress a list of coordinates into a compact string representation.

```csharp
using HerePlatform.Core.Utilities;
```

```csharp
var coordinates = new List<LatLngLiteral>
{
    new(52.5200, 13.4050),
    new(52.5300, 13.4200),
    new(52.5100, 13.3900)
};

string encoded = FlexiblePolyline.Encode(coordinates);
```

The default precision is 5 decimal places. Specify a custom precision when higher or lower accuracy is needed:

```csharp
// Higher precision (7 decimal places)
string encoded = FlexiblePolyline.Encode(coordinates, precision: 7);

// Lower precision for smaller payloads
string encoded = FlexiblePolyline.Encode(coordinates, precision: 3);
```

## Decoding

Use `Decode` to convert a flexible polyline string back into coordinates. This is useful for processing route polylines returned by the Routing Service.

```csharp
List<LatLngLiteral> coordinates = FlexiblePolyline.Decode(encoded);

foreach (var coord in coordinates)
{
    Console.WriteLine($"{coord.Lat}, {coord.Lng}");
}
```

Decoding handles the header format automatically, including version and precision metadata embedded in the encoded string.

**Common use case -- decoding route section polylines:**

```csharp
@inject IRoutingService RoutingService

var result = await RoutingService.CalculateRouteAsync(request);

foreach (var section in result.Routes?.FirstOrDefault()?.Sections ?? [])
{
    // DecodedPolyline is populated automatically by the service,
    // but you can also decode manually from the raw polyline string:
    if (section.Polyline is not null)
    {
        var coords = FlexiblePolyline.Decode(section.Polyline);
        Console.WriteLine($"Decoded {coords.Count} points");
    }
}
```

**Empty or null input** returns an empty list:

```csharp
var empty = FlexiblePolyline.Decode("");
// empty.Count == 0

var nullResult = FlexiblePolyline.Decode(null!);
// returns empty list
```

**Invalid characters** in the input string throw an `ArgumentException`:

```csharp
try
{
    FlexiblePolyline.Decode("invalid!chars");
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
}
```

The codec is compatible with the [HERE Flexible Polyline specification](https://github.com/heremaps/flexible-polyline) and interoperates with encodings produced by other HERE SDKs and APIs.
