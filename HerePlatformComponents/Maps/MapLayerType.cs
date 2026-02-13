using System.Runtime.Serialization;

namespace HerePlatformComponents.Maps;

public enum MapLayerType
{
    // --- Vector base layers (HARP-compatible) ---

    [EnumMember(Value = "vector.normal.map")]
    VectorNormalMap,

    [EnumMember(Value = "vector.normal.mapnight")]
    VectorNormalMapNight,

    [EnumMember(Value = "vector.normal.lite")]
    VectorNormalLite,

    [EnumMember(Value = "vector.normal.litenight")]
    VectorNormalLiteNight,

    [EnumMember(Value = "vector.normal.logistics")]
    VectorNormalLogistics,

    [EnumMember(Value = "vector.normal.logisticsnight")]
    VectorNormalLogisticsNight,

    [EnumMember(Value = "vector.normal.roadnetwork")]
    VectorNormalRoadNetwork,

    [EnumMember(Value = "vector.normal.roadnetworknight")]
    VectorNormalRoadNetworkNight,

    [EnumMember(Value = "vector.normal.topo")]
    VectorNormalTopo,

    [EnumMember(Value = "vector.normal.toponight")]
    VectorNormalTopoNight,

    // --- Vector traffic overlay (add via AddLayerAsync, not SetBaseLayer) ---

    [EnumMember(Value = "vector.traffic.map")]
    VectorTrafficMap,

    // --- Raster layers ---

    [EnumMember(Value = "raster.normal.map")]
    RasterNormalMap,

    [EnumMember(Value = "raster.normal.mapnight")]
    RasterNormalMapNight,

    [EnumMember(Value = "raster.satellite.map")]
    RasterSatelliteMap,

    [EnumMember(Value = "raster.terrain.map")]
    RasterTerrainMap,

    // --- Hybrid layers (satellite + vector, HARP-recommended for satellite views) ---

    [EnumMember(Value = "hybrid.day.raster")]
    HybridDayRaster,

    [EnumMember(Value = "hybrid.day.vector")]
    HybridDayVector,

    [EnumMember(Value = "hybrid.day.traffic")]
    HybridDayTraffic,

    [EnumMember(Value = "hybrid.night.raster")]
    HybridNightRaster,

    [EnumMember(Value = "hybrid.night.vector")]
    HybridNightVector,

    [EnumMember(Value = "hybrid.night.traffic")]
    HybridNightTraffic,

    [EnumMember(Value = "hybrid.liteday.raster")]
    HybridLiteDayRaster,

    [EnumMember(Value = "hybrid.liteday.vector")]
    HybridLiteDayVector,

    [EnumMember(Value = "hybrid.liteday.traffic")]
    HybridLiteDayTraffic,

    [EnumMember(Value = "hybrid.litenight.raster")]
    HybridLiteNightRaster,

    [EnumMember(Value = "hybrid.litenight.vector")]
    HybridLiteNightVector,

    [EnumMember(Value = "hybrid.litenight.traffic")]
    HybridLiteNightTraffic
}
