using HerePlatformComponents.Maps.Coordinates;

namespace HerePlatformComponents.Tests.Coordinates;

[TestFixture]
public class PointTests
{
    [Test]
    public void DefaultConstructor_AllZero()
    {
        var point = new Point();

        Assert.That(point.X, Is.EqualTo(0));
        Assert.That(point.Y, Is.EqualTo(0));
    }

    [Test]
    public void ParameterizedConstructor_SetsXY()
    {
        var point = new Point(100.5, 200.3);

        Assert.That(point.X, Is.EqualTo(100.5));
        Assert.That(point.Y, Is.EqualTo(200.3));
    }

    [Test]
    public void Properties_AreSettable()
    {
        var point = new Point { X = 42.0, Y = 84.0 };

        Assert.That(point.X, Is.EqualTo(42.0));
        Assert.That(point.Y, Is.EqualTo(84.0));
    }

    [Test]
    public void NegativeValues_AreAllowed()
    {
        var point = new Point(-50, -100);

        Assert.That(point.X, Is.EqualTo(-50));
        Assert.That(point.Y, Is.EqualTo(-100));
    }
}
