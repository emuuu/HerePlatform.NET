using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Coordinates;

[TestFixture]
public class LatLngLiteralTests
{
    [Test]
    public void Constructor_ValidCoordinates_SetsProperties()
    {
        var coord = new LatLngLiteral(52.52, 13.405);

        Assert.That(coord.Lat, Is.EqualTo(52.52));
        Assert.That(coord.Lng, Is.EqualTo(13.405));
    }

    [Test]
    public void Constructor_ZeroCoordinates_Succeeds()
    {
        var coord = new LatLngLiteral(0, 0);

        Assert.That(coord.Lat, Is.EqualTo(0));
        Assert.That(coord.Lng, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_BoundaryValues_Succeeds()
    {
        var min = new LatLngLiteral(-90, -180);
        var max = new LatLngLiteral(90, 180);

        Assert.That(min.Lat, Is.EqualTo(-90));
        Assert.That(min.Lng, Is.EqualTo(-180));
        Assert.That(max.Lat, Is.EqualTo(90));
        Assert.That(max.Lng, Is.EqualTo(180));
    }

    [TestCase(-91, 0)]
    [TestCase(91, 0)]
    [TestCase(-90.001, 0)]
    [TestCase(90.001, 0)]
    public void Constructor_InvalidLatitude_ThrowsArgumentException(double lat, double lng)
    {
        Assert.Throws<ArgumentException>(() => new LatLngLiteral(lat, lng));
    }

    [TestCase(0, -181)]
    [TestCase(0, 181)]
    [TestCase(0, -180.001)]
    [TestCase(0, 180.001)]
    public void Constructor_InvalidLongitude_ThrowsArgumentException(double lat, double lng)
    {
        Assert.Throws<ArgumentException>(() => new LatLngLiteral(lat, lng));
    }

    [Test]
    public void Equals_SameValues_ReturnsTrue()
    {
        var a = new LatLngLiteral(52.52, 13.405);
        var b = new LatLngLiteral(52.52, 13.405);

        Assert.That(a.Equals(b), Is.True);
        Assert.That(a == b, Is.True);
        Assert.That(a != b, Is.False);
    }

    [Test]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var a = new LatLngLiteral(52.52, 13.405);
        var b = new LatLngLiteral(48.8566, 2.3522);

        Assert.That(a.Equals(b), Is.False);
        Assert.That(a == b, Is.False);
        Assert.That(a != b, Is.True);
    }

    [Test]
    public void Equals_DifferentLatOnly_ReturnsFalse()
    {
        var a = new LatLngLiteral(52.52, 13.405);
        var b = new LatLngLiteral(52.53, 13.405);

        Assert.That(a.Equals(b), Is.False);
    }

    [Test]
    public void Equals_DifferentLngOnly_ReturnsFalse()
    {
        var a = new LatLngLiteral(52.52, 13.405);
        var b = new LatLngLiteral(52.52, 13.406);

        Assert.That(a.Equals(b), Is.False);
    }

    [Test]
    public void Equals_BoxedObject_ReturnsTrue()
    {
        var a = new LatLngLiteral(52.52, 13.405);
        object b = new LatLngLiteral(52.52, 13.405);

        Assert.That(a.Equals(b), Is.True);
    }

    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        var a = new LatLngLiteral(52.52, 13.405);

        Assert.That(a.Equals(null), Is.False);
    }

    [Test]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var a = new LatLngLiteral(52.52, 13.405);

        Assert.That(a.Equals("not a coord"), Is.False);
    }

    [Test]
    public void GetHashCode_SameValues_SameHash()
    {
        var a = new LatLngLiteral(52.52, 13.405);
        var b = new LatLngLiteral(52.52, 13.405);

        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }

    [Test]
    public void GetHashCode_DifferentValues_DifferentHash()
    {
        var a = new LatLngLiteral(52.52, 13.405);
        var b = new LatLngLiteral(48.8566, 2.3522);

        Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
    }

    [Test]
    public void Default_IsZeroZero()
    {
        var coord = default(LatLngLiteral);

        Assert.That(coord.Lat, Is.EqualTo(0));
        Assert.That(coord.Lng, Is.EqualTo(0));
    }

    [Test]
    public void CanBeUsedAsDictionaryKey()
    {
        var dict = new Dictionary<LatLngLiteral, string>
        {
            [new LatLngLiteral(52.52, 13.405)] = "Berlin",
            [new LatLngLiteral(48.8566, 2.3522)] = "Paris"
        };

        Assert.That(dict[new LatLngLiteral(52.52, 13.405)], Is.EqualTo("Berlin"));
        Assert.That(dict[new LatLngLiteral(48.8566, 2.3522)], Is.EqualTo("Paris"));
    }
}
