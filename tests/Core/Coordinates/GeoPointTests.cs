using HerePlatformComponents.Maps.Coordinates;

namespace HerePlatformComponents.Tests.Coordinates;

[TestFixture]
public class GeoPointTests
{
    [Test]
    public void DefaultConstructor_AllZero()
    {
        var point = new GeoPoint();

        Assert.That(point.Lat, Is.EqualTo(0));
        Assert.That(point.Lng, Is.EqualTo(0));
        Assert.That(point.Alt, Is.Null);
    }

    [Test]
    public void Constructor_WithoutAltitude_SetsLatLng()
    {
        var point = new GeoPoint(52.52, 13.405);

        Assert.That(point.Lat, Is.EqualTo(52.52));
        Assert.That(point.Lng, Is.EqualTo(13.405));
        Assert.That(point.Alt, Is.Null);
    }

    [Test]
    public void Constructor_WithAltitude_SetsAll()
    {
        var point = new GeoPoint(52.52, 13.405, 150.0);

        Assert.That(point.Lat, Is.EqualTo(52.52));
        Assert.That(point.Lng, Is.EqualTo(13.405));
        Assert.That(point.Alt, Is.EqualTo(150.0));
    }

    [Test]
    public void Properties_AreSettable()
    {
        var point = new GeoPoint { Lat = 1.0, Lng = 2.0, Alt = 3.0 };

        Assert.That(point.Lat, Is.EqualTo(1.0));
        Assert.That(point.Lng, Is.EqualTo(2.0));
        Assert.That(point.Alt, Is.EqualTo(3.0));
    }
}
