using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class MapViewChangeEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new MapViewChangeEventArgs();

        Assert.That(args.Center, Is.Null);
        Assert.That(args.Zoom, Is.EqualTo(0));
        Assert.That(args.Tilt, Is.EqualTo(0));
        Assert.That(args.Heading, Is.EqualTo(0));
        Assert.That(args.Type, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var center = new LatLngLiteral(52.52, 13.405);
        var args = new MapViewChangeEventArgs
        {
            Center = center,
            Zoom = 14.5,
            Tilt = 30.0,
            Heading = 45.0,
            Type = "mapviewchangeend"
        };

        Assert.That(args.Center, Is.EqualTo(center));
        Assert.That(args.Zoom, Is.EqualTo(14.5));
        Assert.That(args.Tilt, Is.EqualTo(30.0));
        Assert.That(args.Heading, Is.EqualTo(45.0));
        Assert.That(args.Type, Is.EqualTo("mapviewchangeend"));
    }

    [TestCase("mapviewchange")]
    [TestCase("mapviewchangestart")]
    [TestCase("mapviewchangeend")]
    public void Type_AcceptsAllViewChangeEventNames(string eventType)
    {
        var args = new MapViewChangeEventArgs { Type = eventType };

        Assert.That(args.Type, Is.EqualTo(eventType));
    }
}
