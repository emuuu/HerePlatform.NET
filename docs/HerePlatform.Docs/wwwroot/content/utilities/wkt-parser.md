---
title: WKT Parser
category: Utilities
order: 1
description: "Parse Well-Known Text geometry strings."
apiRef: WktParser
---

## Parsing

`WktParser` is a static utility class that converts Well-Known Text (WKT) geometry strings into `LatLngLiteral` coordinates. WKT uses longitude-first ordering; the parser handles the conversion to `LatLngLiteral(lat, lng)` automatically.

```csharp
using HerePlatform.Core.Utilities;
```

**Parse a POINT:**

```csharp
var point = WktParser.ParsePoint("POINT(13.405 52.52)");
if (point.HasValue)
{
    Console.WriteLine($"Lat: {point.Value.Lat}, Lng: {point.Value.Lng}");
}
```

**Parse a LINESTRING:**

```csharp
var line = WktParser.ParseLineString(
    "LINESTRING(13.38 52.53, 13.42 52.50, 13.45 52.52)");

foreach (var coord in line)
{
    Console.WriteLine($"{coord.Lat}, {coord.Lng}");
}
```

**Parse a POLYGON:**

```csharp
var rings = WktParser.ParsePolygon(
    "POLYGON((13.38 52.53, 13.42 52.53, 13.42 52.50, 13.38 52.50, 13.38 52.53))");

var exteriorRing = rings.FirstOrDefault();
```

**Parse a MULTIPOINT:**

```csharp
var points = WktParser.ParseMultiPoint(
    "MULTIPOINT((13.405 52.52),(2.3522 48.8566))");
```

**Parse a MULTILINESTRING:**

```csharp
var lines = WktParser.ParseMultiLineString(
    "MULTILINESTRING((13.38 52.53, 13.42 52.50),(2.35 48.86, 2.29 48.85))");
```

## Supported Geometry Types

| Method | WKT Type | Return Type |
|--------|----------|-------------|
| `ParsePoint` | `POINT` | `LatLngLiteral?` |
| `ParseLineString` | `LINESTRING` | `List<LatLngLiteral>` |
| `ParsePolygon` | `POLYGON` | `List<List<LatLngLiteral>>` (exterior ring + holes) |
| `ParseMultiPoint` | `MULTIPOINT` | `List<LatLngLiteral>` |
| `ParseMultiLineString` | `MULTILINESTRING` | `List<List<LatLngLiteral>>` |

All methods return empty collections (or `null` for `ParsePoint`) when the input is invalid or does not match the expected geometry type. Parsing is case-insensitive for the geometry type keyword.

```csharp
// Returns null for invalid input
var invalid = WktParser.ParsePoint("not a valid WKT string");
// invalid == null

// Returns empty list for wrong type
var empty = WktParser.ParseLineString("POINT(13.405 52.52)");
// empty.Count == 0
```
