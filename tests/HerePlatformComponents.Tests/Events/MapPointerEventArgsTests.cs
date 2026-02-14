using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class MapPointerEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new MapPointerEventArgs();

        Assert.That(args.ViewportX, Is.EqualTo(0));
        Assert.That(args.ViewportY, Is.EqualTo(0));
        Assert.That(args.Position, Is.Null);
        Assert.That(args.Button, Is.EqualTo(0));
        Assert.That(args.Buttons, Is.EqualTo(0));
        Assert.That(args.PointerType, Is.Null);
        Assert.That(args.Type, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var position = new LatLngLiteral(52.52, 13.405);
        var args = new MapPointerEventArgs
        {
            ViewportX = 400.5,
            ViewportY = 300.25,
            Position = position,
            Button = 2,
            Buttons = 3,
            PointerType = "mouse",
            Type = "tap"
        };

        Assert.That(args.ViewportX, Is.EqualTo(400.5));
        Assert.That(args.ViewportY, Is.EqualTo(300.25));
        Assert.That(args.Position, Is.EqualTo(position));
        Assert.That(args.Button, Is.EqualTo(2));
        Assert.That(args.Buttons, Is.EqualTo(3));
        Assert.That(args.PointerType, Is.EqualTo("mouse"));
        Assert.That(args.Type, Is.EqualTo("tap"));
    }

    [Test]
    public void Position_IsNullable()
    {
        var args = new MapPointerEventArgs
        {
            ViewportX = 100,
            ViewportY = 200,
            Position = null
        };

        Assert.That(args.Position, Is.Null);
        Assert.That(args.Position.HasValue, Is.False);
    }

    [TestCase("tap")]
    [TestCase("dbltap")]
    [TestCase("longpress")]
    [TestCase("contextmenu")]
    [TestCase("pointerdown")]
    [TestCase("pointerup")]
    [TestCase("pointermove")]
    [TestCase("pointerenter")]
    [TestCase("pointerleave")]
    public void Type_AcceptsAllPointerEventNames(string eventType)
    {
        var args = new MapPointerEventArgs { Type = eventType };

        Assert.That(args.Type, Is.EqualTo(eventType));
    }

    [TestCase("mouse")]
    [TestCase("touch")]
    [TestCase("pen")]
    public void PointerType_AcceptsAllTypes(string pointerType)
    {
        var args = new MapPointerEventArgs { PointerType = pointerType };

        Assert.That(args.PointerType, Is.EqualTo(pointerType));
    }
}
