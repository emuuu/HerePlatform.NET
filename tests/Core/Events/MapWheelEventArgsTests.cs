using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class MapWheelEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new MapWheelEventArgs();

        Assert.That(args.Delta, Is.EqualTo(0));
        Assert.That(args.ViewportX, Is.EqualTo(0));
        Assert.That(args.ViewportY, Is.EqualTo(0));
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var args = new MapWheelEventArgs
        {
            Delta = -120.0,
            ViewportX = 512.0,
            ViewportY = 384.0
        };

        Assert.That(args.Delta, Is.EqualTo(-120.0));
        Assert.That(args.ViewportX, Is.EqualTo(512.0));
        Assert.That(args.ViewportY, Is.EqualTo(384.0));
    }

    [Test]
    public void Delta_AcceptsPositiveValues()
    {
        var args = new MapWheelEventArgs { Delta = 100.0 };

        Assert.That(args.Delta, Is.EqualTo(100.0));
    }

    [Test]
    public void Delta_AcceptsNegativeValues()
    {
        var args = new MapWheelEventArgs { Delta = -100.0 };

        Assert.That(args.Delta, Is.EqualTo(-100.0));
    }
}
