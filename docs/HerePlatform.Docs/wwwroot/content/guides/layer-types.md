---
title: Layer Types
category: Guides
order: 1
description: "Understanding HERE map layer types."
---

## Normal

Standard vector map layers rendered with the HARP engine. Best for general-purpose maps.

```csharp
var options = new MapOptions
{
    LayerType = MapLayerType.VectorNormalMap,  // Default day mode
    Center = new LatLngLiteral(52.52, 13.405),
    Zoom = 12
};
```

Night variants are available for dark themes:

```csharp
options.LayerType = MapLayerType.VectorNormalMapNight;
```

## Satellite

Raster satellite imagery. Provides aerial/satellite photography without road labels.

```csharp
options.LayerType = MapLayerType.RasterSatelliteMap;
```

## Hybrid

Hybrid layers combine satellite imagery with vector road overlays. These are composites of multiple layers:

- `HybridDayRaster` -- satellite imagery base (set as base layer)
- `HybridDayVector` -- roads and labels overlay (added on top)
- `HybridDayTraffic` -- traffic overlay (added on top)

```csharp
// Set satellite as base, add road labels as overlay
var options = new MapOptions
{
    LayerType = MapLayerType.HybridDayRaster
};
```

Night and lite variants are also available: `HybridNightRaster`, `HybridLiteDayRaster`, `HybridLiteNightRaster`, each with corresponding vector and traffic overlays.

## Terrain

Raster terrain map showing elevation and topographic features.

```csharp
options.LayerType = MapLayerType.RasterTerrainMap;
```

## Traffic Layers

Add real-time traffic as a vector overlay on top of any base layer:

```csharp
// Via MapLayerType (as base, not recommended alone)
options.LayerType = MapLayerType.VectorTrafficMap;

// Better: add as overlay via AdvancedHereMap
await mapRef.AddLayerAsync("vector.traffic.map");

// Remove when no longer needed
await mapRef.RemoveLayerAsync("vector.traffic.map");
```

## MapLayerType Enum Values

| Value | Description |
|-------|-------------|
| `VectorNormalMap` | Standard vector map (day). |
| `VectorNormalMapNight` | Standard vector map (night). |
| `VectorNormalLite` | Simplified vector map (day). |
| `VectorNormalLiteNight` | Simplified vector map (night). |
| `VectorNormalLogistics` | Logistics-optimized map (day). |
| `VectorNormalLogisticsNight` | Logistics-optimized map (night). |
| `VectorNormalRoadNetwork` | Road network only (day). |
| `VectorNormalRoadNetworkNight` | Road network only (night). |
| `VectorNormalTopo` | Topographic vector map (day). |
| `VectorNormalTopoNight` | Topographic vector map (night). |
| `VectorTrafficMap` | Traffic vector overlay. |
| `RasterNormalMap` | Raster map (day). |
| `RasterNormalMapNight` | Raster map (night). |
| `RasterSatelliteMap` | Satellite imagery. |
| `RasterTerrainMap` | Terrain/elevation map. |
| `HybridDayRaster` | Satellite base (day). |
| `HybridDayVector` | Road labels overlay (day). |
| `HybridDayTraffic` | Traffic overlay on satellite (day). |
| `HybridNightRaster` | Satellite base (night). |
| `HybridNightVector` | Road labels overlay (night). |
| `HybridNightTraffic` | Traffic overlay on satellite (night). |
| `HybridLiteDayRaster` | Satellite base, lite labels (day). |
| `HybridLiteDayVector` | Lite road labels overlay (day). |
| `HybridLiteDayTraffic` | Lite traffic overlay (day). |
| `HybridLiteNightRaster` | Satellite base, lite labels (night). |
| `HybridLiteNightVector` | Lite road labels overlay (night). |
| `HybridLiteNightTraffic` | Lite traffic overlay (night). |
