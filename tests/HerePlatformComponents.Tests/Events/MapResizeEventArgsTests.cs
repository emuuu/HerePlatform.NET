using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class MapResizeEventArgsTests
{
    [Test]
    public void DefaultValues_AreZero()
    {
        var args = new MapResizeEventArgs();

        Assert.That(args.Width, Is.EqualTo(0.0));
        Assert.That(args.Height, Is.EqualTo(0.0));
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var args = new MapResizeEventArgs
        {
            Width = 800.0,
            Height = 600.0
        };

        Assert.That(args.Width, Is.EqualTo(800.0));
        Assert.That(args.Height, Is.EqualTo(600.0));
    }
}
