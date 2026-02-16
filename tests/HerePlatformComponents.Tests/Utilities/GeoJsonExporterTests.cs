using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Utilities;

namespace HerePlatformComponents.Tests.Utilities;

[TestFixture]
public class GeoJsonExporterTests
{
    [Test]
    public void ToGeoJsonFeature_Point()
    {
        var point = new LatLngLiteral(52.52, 13.405);
        var result = GeoJsonExporter.ToGeoJsonFeature(point);

        Assert.That(result, Does.Contain("\"type\":\"Feature\""));
        Assert.That(result, Does.Contain("\"type\":\"Point\""));
        Assert.That(result, Does.Contain("13.405"));
        Assert.That(result, Does.Contain("52.52"));
    }

    [Test]
    public void ToGeoJsonFeature_Point_WithProperties()
    {
        var point = new LatLngLiteral(52.52, 13.405);
        var props = new Dictionary<string, object>
        {
            { "name", "Berlin" },
            { "population", 3645000 }
        };

        var result = GeoJsonExporter.ToGeoJsonFeature(point, props);

        Assert.That(result, Does.Contain("\"name\":\"Berlin\""));
        Assert.That(result, Does.Contain("\"population\":3645000"));
    }

    [Test]
    public void ToGeoJsonFeature_LineString()
    {
        var line = new List<LatLngLiteral>
        {
            new(52.52, 13.405),
            new(48.8566, 2.3522)
        };

        var result = GeoJsonExporter.ToLineStringFeature(line);

        Assert.That(result, Does.Contain("\"type\":\"LineString\""));
        Assert.That(result, Does.Contain("[13.405,52.52]"));
        Assert.That(result, Does.Contain("[2.3522,48.8566]"));
    }

    [Test]
    public void ToGeoJsonFeature_Polygon()
    {
        var exterior = new List<LatLngLiteral>
        {
            new(52.55, 13.35),
            new(52.55, 13.45),
            new(52.49, 13.45),
            new(52.49, 13.35),
            new(52.55, 13.35)
        };

        var result = GeoJsonExporter.ToPolygonFeature(exterior);

        Assert.That(result, Does.Contain("\"type\":\"Polygon\""));
    }

    [Test]
    public void ToPolygonFeature_WithHoles()
    {
        var exterior = new List<LatLngLiteral>
        {
            new(0, 0), new(0, 10), new(10, 10), new(10, 0), new(0, 0)
        };
        var holes = new List<List<LatLngLiteral>>
        {
            new List<LatLngLiteral>
            {
                new(2, 2), new(2, 8), new(8, 8), new(8, 2), new(2, 2)
            }
        };

        var result = GeoJsonExporter.ToPolygonFeature(exterior, holes);

        Assert.That(result, Does.Contain("\"type\":\"Polygon\""));
        // Should have two coordinate arrays (exterior + 1 hole)
        var coordCount = result.Split(new[] { "],[" }, StringSplitOptions.None).Length;
        Assert.That(coordCount, Is.GreaterThan(2));
    }

    [Test]
    public void ToFeatureCollection()
    {
        var f1 = GeoJsonExporter.ToGeoJsonFeature(new LatLngLiteral(52.52, 13.405));
        var f2 = GeoJsonExporter.ToGeoJsonFeature(new LatLngLiteral(48.8566, 2.3522));

        var result = GeoJsonExporter.ToFeatureCollection(new[] { f1, f2 });

        Assert.That(result, Does.Contain("\"type\":\"FeatureCollection\""));
        Assert.That(result, Does.Contain("\"features\":["));
        Assert.That(result, Does.Contain("13.405"));
        Assert.That(result, Does.Contain("2.3522"));
    }

    [Test]
    public void ToFeatureCollection_Empty()
    {
        var result = GeoJsonExporter.ToFeatureCollection(Array.Empty<string>());

        Assert.That(result, Is.EqualTo("{\"type\":\"FeatureCollection\",\"features\":[]}"));
    }

    [Test]
    public void ToGeoJsonFeature_Point_NoProperties()
    {
        var point = new LatLngLiteral(0, 0);
        var result = GeoJsonExporter.ToGeoJsonFeature(point);

        Assert.That(result, Does.Contain("\"properties\":{}"));
    }
}
