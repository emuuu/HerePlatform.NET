using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class MapDragEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new MapDragEventArgs();

        Assert.That(args.ViewportX, Is.EqualTo(0));
        Assert.That(args.ViewportY, Is.EqualTo(0));
        Assert.That(args.Position, Is.Null);
        Assert.That(args.Type, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var position = new LatLngLiteral(48.8566, 2.3522);
        var args = new MapDragEventArgs
        {
            ViewportX = 250.0,
            ViewportY = 175.5,
            Position = position,
            Type = "dragend"
        };

        Assert.That(args.ViewportX, Is.EqualTo(250.0));
        Assert.That(args.ViewportY, Is.EqualTo(175.5));
        Assert.That(args.Position, Is.EqualTo(position));
        Assert.That(args.Type, Is.EqualTo("dragend"));
    }

    [Test]
    public void Position_IsNullable()
    {
        var args = new MapDragEventArgs
        {
            ViewportX = 100,
            ViewportY = 200,
            Position = null
        };

        Assert.That(args.Position, Is.Null);
        Assert.That(args.Position.HasValue, Is.False);
    }

    [TestCase("dragstart")]
    [TestCase("drag")]
    [TestCase("dragend")]
    public void Type_AcceptsAllDragEventNames(string eventType)
    {
        var args = new MapDragEventArgs { Type = eventType };

        Assert.That(args.Type, Is.EqualTo(eventType));
    }
}
