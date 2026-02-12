using System.Runtime.Serialization;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class MapLayerTypeTests
{
    [Test]
    public void AllEnumValues_Exist()
    {
        var values = Enum.GetValues<MapLayerType>();

        Assert.That(values, Has.Length.EqualTo(30));

        // Vector base layers
        Assert.That(values, Does.Contain(MapLayerType.VectorNormalMap));
        Assert.That(values, Does.Contain(MapLayerType.VectorNormalMapNight));
        Assert.That(values, Does.Contain(MapLayerType.VectorNormalLite));
        Assert.That(values, Does.Contain(MapLayerType.VectorNormalLogistics));
        Assert.That(values, Does.Contain(MapLayerType.VectorNormalTopo));

        // Raster layers
        Assert.That(values, Does.Contain(MapLayerType.RasterNormalMap));
        Assert.That(values, Does.Contain(MapLayerType.RasterSatelliteMap));
        Assert.That(values, Does.Contain(MapLayerType.RasterTerrainMap));

        // Hybrid layers
        Assert.That(values, Does.Contain(MapLayerType.HybridDayRaster));
        Assert.That(values, Does.Contain(MapLayerType.HybridDayVector));
        Assert.That(values, Does.Contain(MapLayerType.HybridNightRaster));
    }

    [TestCase(MapLayerType.VectorNormalMap, "vector.normal.map")]
    [TestCase(MapLayerType.VectorNormalMapNight, "vector.normal.mapnight")]
    [TestCase(MapLayerType.VectorNormalLite, "vector.normal.lite")]
    [TestCase(MapLayerType.VectorNormalLiteNight, "vector.normal.litenight")]
    [TestCase(MapLayerType.VectorNormalLogistics, "vector.normal.logistics")]
    [TestCase(MapLayerType.VectorNormalRoadNetwork, "vector.normal.roadnetwork")]
    [TestCase(MapLayerType.VectorNormalTopo, "vector.normal.topo")]
    [TestCase(MapLayerType.VectorTrafficMap, "vector.traffic.map")]
    [TestCase(MapLayerType.RasterNormalMap, "raster.normal.map")]
    [TestCase(MapLayerType.RasterSatelliteMap, "raster.satellite.map")]
    [TestCase(MapLayerType.RasterTerrainMap, "raster.terrain.map")]
    [TestCase(MapLayerType.HybridDayRaster, "hybrid.day.raster")]
    [TestCase(MapLayerType.HybridDayVector, "hybrid.day.vector")]
    [TestCase(MapLayerType.HybridLiteDayRaster, "hybrid.liteday.raster")]
    public void EnumMemberAttribute_HasCorrectValue(MapLayerType layerType, string expectedValue)
    {
        var memberInfo = typeof(MapLayerType).GetField(layerType.ToString())!;
        var attribute = (EnumMemberAttribute)memberInfo
            .GetCustomAttributes(typeof(EnumMemberAttribute), false)
            .Single();

        Assert.That(attribute.Value, Is.EqualTo(expectedValue));
    }
}
