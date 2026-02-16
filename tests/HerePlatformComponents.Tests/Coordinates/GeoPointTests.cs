using HerePlatform.Core.Coordinates;

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

    [Test]
    public void Constructor_BoundaryValues_Succeeds()
    {
        var min = new GeoPoint(-90, -180);
        var max = new GeoPoint(90, 180);

        Assert.That(min.Lat, Is.EqualTo(-90));
        Assert.That(min.Lng, Is.EqualTo(-180));
        Assert.That(max.Lat, Is.EqualTo(90));
        Assert.That(max.Lng, Is.EqualTo(180));
    }

    [TestCase(-91)]
    [TestCase(91)]
    [TestCase(-90.001)]
    [TestCase(90.001)]
    public void Constructor_InvalidLatitude_ThrowsArgumentOutOfRangeException(double lat)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GeoPoint(lat, 0));
    }

    [TestCase(-181)]
    [TestCase(181)]
    [TestCase(-180.001)]
    [TestCase(180.001)]
    public void Constructor_InvalidLongitude_ThrowsArgumentOutOfRangeException(double lng)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GeoPoint(0, lng));
    }

    [TestCase(-91)]
    [TestCase(91)]
    public void LatSetter_InvalidValue_ThrowsArgumentOutOfRangeException(double lat)
    {
        var point = new GeoPoint();
        Assert.Throws<ArgumentOutOfRangeException>(() => point.Lat = lat);
    }

    [TestCase(-181)]
    [TestCase(181)]
    public void LngSetter_InvalidValue_ThrowsArgumentOutOfRangeException(double lng)
    {
        var point = new GeoPoint();
        Assert.Throws<ArgumentOutOfRangeException>(() => point.Lng = lng);
    }
}
