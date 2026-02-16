using HerePlatform.Core.Coordinates;

namespace HerePlatformComponents.Tests.Coordinates;

[TestFixture]
public class GeoRectTests
{
    [Test]
    public void DefaultConstructor_AllZero()
    {
        var rect = new GeoRect();

        Assert.That(rect.Top, Is.EqualTo(0));
        Assert.That(rect.Left, Is.EqualTo(0));
        Assert.That(rect.Bottom, Is.EqualTo(0));
        Assert.That(rect.Right, Is.EqualTo(0));
    }

    [Test]
    public void ParameterizedConstructor_SetsAllProperties()
    {
        var rect = new GeoRect(52.6, 13.1, 52.4, 13.8);

        Assert.That(rect.Top, Is.EqualTo(52.6));
        Assert.That(rect.Left, Is.EqualTo(13.1));
        Assert.That(rect.Bottom, Is.EqualTo(52.4));
        Assert.That(rect.Right, Is.EqualTo(13.8));
    }

    [Test]
    public void Properties_AreSettable()
    {
        var rect = new GeoRect
        {
            Top = 10.5,
            Left = 20.3,
            Bottom = 5.1,
            Right = 25.7
        };

        Assert.That(rect.Top, Is.EqualTo(10.5));
        Assert.That(rect.Left, Is.EqualTo(20.3));
        Assert.That(rect.Bottom, Is.EqualTo(5.1));
        Assert.That(rect.Right, Is.EqualTo(25.7));
    }

    [Test]
    public void NegativeCoordinates_AreAllowed()
    {
        var rect = new GeoRect(-10, -20, -30, -5);

        Assert.That(rect.Top, Is.EqualTo(-10));
        Assert.That(rect.Left, Is.EqualTo(-20));
        Assert.That(rect.Bottom, Is.EqualTo(-30));
        Assert.That(rect.Right, Is.EqualTo(-5));
    }
}
