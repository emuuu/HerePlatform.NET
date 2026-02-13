using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Utilities;

namespace HerePlatformComponents.Tests.Utilities;

[TestFixture]
public class WktParserTests
{
    [Test]
    public void ParsePoint_ValidWkt()
    {
        var result = WktParser.ParsePoint("POINT(13.405 52.52)");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value.Lat, Is.EqualTo(52.52).Within(0.00001));
        Assert.That(result.Value.Lng, Is.EqualTo(13.405).Within(0.00001));
    }

    [Test]
    public void ParsePoint_NullReturnsNull()
    {
        var result = WktParser.ParsePoint(null!);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParsePoint_EmptyReturnsNull()
    {
        var result = WktParser.ParsePoint("");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParsePoint_InvalidPrefix_ReturnsNull()
    {
        var result = WktParser.ParsePoint("LINESTRING(1 2, 3 4)");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseLineString_ValidWkt()
    {
        var result = WktParser.ParseLineString("LINESTRING(13.405 52.52, 2.3522 48.8566)");

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Lat, Is.EqualTo(52.52).Within(0.00001));
        Assert.That(result[0].Lng, Is.EqualTo(13.405).Within(0.00001));
        Assert.That(result[1].Lat, Is.EqualTo(48.8566).Within(0.00001));
    }

    [Test]
    public void ParseLineString_EmptyReturnsEmpty()
    {
        var result = WktParser.ParseLineString("");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParsePolygon_ValidWkt()
    {
        var result = WktParser.ParsePolygon(
            "POLYGON((13.35 52.55, 13.45 52.55, 13.45 52.49, 13.35 52.49, 13.35 52.55))");

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Has.Count.EqualTo(5));
        Assert.That(result[0][0].Lat, Is.EqualTo(52.55).Within(0.00001));
    }

    [Test]
    public void ParsePolygon_WithHoles()
    {
        var result = WktParser.ParsePolygon(
            "POLYGON((0 0, 10 0, 10 10, 0 10, 0 0),(2 2, 8 2, 8 8, 2 8, 2 2))");

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Has.Count.EqualTo(5));
        Assert.That(result[1], Has.Count.EqualTo(5));
    }

    [Test]
    public void ParsePolygon_EmptyReturnsEmpty()
    {
        var result = WktParser.ParsePolygon("");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseMultiPoint_ValidWkt()
    {
        var result = WktParser.ParseMultiPoint("MULTIPOINT((13.405 52.52),(2.3522 48.8566))");

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Lat, Is.EqualTo(52.52).Within(0.00001));
        Assert.That(result[1].Lat, Is.EqualTo(48.8566).Within(0.00001));
    }

    [Test]
    public void ParseMultiLineString_ValidWkt()
    {
        var result = WktParser.ParseMultiLineString(
            "MULTILINESTRING((0 0, 1 1, 2 2),(3 3, 4 4))");

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Has.Count.EqualTo(3));
        Assert.That(result[1], Has.Count.EqualTo(2));
    }

    [Test]
    public void ParsePoint_NegativeCoordinates()
    {
        var result = WktParser.ParsePoint("POINT(-43.1729 -22.9068)");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value.Lat, Is.EqualTo(-22.9068).Within(0.00001));
        Assert.That(result.Value.Lng, Is.EqualTo(-43.1729).Within(0.00001));
    }
}
