using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class ContextMenuEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new ContextMenuEventArgs();

        Assert.That(args.Position, Is.Null);
        Assert.That(args.ItemLabel, Is.Null);
        Assert.That(args.ItemData, Is.Null);
        Assert.That(args.ViewportX, Is.EqualTo(0));
        Assert.That(args.ViewportY, Is.EqualTo(0));
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var args = new ContextMenuEventArgs
        {
            Position = new LatLngLiteral(52.52, 13.405),
            ItemLabel = "Add Marker",
            ItemData = "custom-data",
            ViewportX = 100.5,
            ViewportY = 200.3
        };

        Assert.That(args.Position!.Value.Lat, Is.EqualTo(52.52));
        Assert.That(args.ItemLabel, Is.EqualTo("Add Marker"));
        Assert.That(args.ItemData, Is.EqualTo("custom-data"));
        Assert.That(args.ViewportX, Is.EqualTo(100.5));
        Assert.That(args.ViewportY, Is.EqualTo(200.3));
    }
}
