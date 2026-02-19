---
title: GeoJSON Exporter
category: Utilities
order: 2
description: "Export map objects to GeoJSON format."
apiRef: GeoJsonExporter
---

## Export Features

`GeoJsonExporter` is a static utility class that converts coordinates into GeoJSON Feature strings. All methods return raw JSON strings.

```csharp
using HerePlatform.Core.Utilities;
```

**Point Feature:**

```csharp
var json = GeoJsonExporter.ToGeoJsonFeature(
    new LatLngLiteral(52.52, 13.405));
// {"type":"Feature","geometry":{"type":"Point","coordinates":[13.405,52.52]},"properties":{}}
```

**Point with properties:**

```csharp
var props = new Dictionary<string, object>
{
    { "name", "Berlin" },
    { "population", 3645000 }
};

var json = GeoJsonExporter.ToGeoJsonFeature(
    new LatLngLiteral(52.52, 13.405), props);
```

**LineString Feature:**

```csharp
var route = new List<LatLngLiteral>
{
    new(52.52, 13.405),
    new(52.53, 13.42),
    new(52.50, 13.39)
};

var json = GeoJsonExporter.ToLineStringFeature(route,
    new Dictionary<string, object> { { "name", "Route A" } });
```

**Polygon Feature:**

```csharp
var exterior = new List<LatLngLiteral>
{
    new(52.53, 13.38), new(52.53, 13.42),
    new(52.50, 13.42), new(52.50, 13.38),
    new(52.53, 13.38) // closed ring
};

var json = GeoJsonExporter.ToPolygonFeature(exterior);
```

**Polygon with holes:**

```csharp
var hole = new List<LatLngLiteral>
{
    new(52.52, 13.39), new(52.52, 13.41),
    new(52.51, 13.41), new(52.51, 13.39),
    new(52.52, 13.39)
};

var json = GeoJsonExporter.ToPolygonFeature(exterior, holes: [hole]);
```

## Export FeatureCollection

Combine multiple feature JSON strings into a GeoJSON FeatureCollection.

```csharp
var point = GeoJsonExporter.ToGeoJsonFeature(new LatLngLiteral(52.52, 13.405));
var line = GeoJsonExporter.ToLineStringFeature(route);
var polygon = GeoJsonExporter.ToPolygonFeature(exterior);

var collection = GeoJsonExporter.ToFeatureCollection([point, line, polygon]);
// {"type":"FeatureCollection","features":[...]}
```

All coordinate output follows the GeoJSON specification: `[longitude, latitude]`. Property values support `string`, `bool`, `int`, `double`, and `null` types. Other types are serialized via `ToString()`.

```csharp
var props = new Dictionary<string, object>
{
    { "label", "Warehouse" },
    { "active", true },
    { "capacity", 500 },
    { "rating", 4.7 }
};

var json = GeoJsonExporter.ToGeoJsonFeature(
    new LatLngLiteral(52.52, 13.405), props);
```
