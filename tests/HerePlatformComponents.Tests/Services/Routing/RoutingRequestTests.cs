using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Services.Routing;

[TestFixture]
public class RoutingRequestTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var request = new RoutingRequest();

        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Car));
        Assert.That(request.RoutingMode, Is.EqualTo(RoutingMode.Fast));
        Assert.That(request.ReturnPolyline, Is.True);
        Assert.That(request.Alternatives, Is.EqualTo(0));
        Assert.That(request.Avoid, Is.EqualTo(RoutingAvoidFeature.None));
        Assert.That(request.Via, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var origin = new LatLngLiteral(52.52, 13.405);
        var dest = new LatLngLiteral(48.8566, 2.3522);
        var via = new List<LatLngLiteral> { new(50.0, 8.0) };

        var request = new RoutingRequest
        {
            Origin = origin,
            Destination = dest,
            Via = via,
            TransportMode = TransportMode.Pedestrian,
            RoutingMode = RoutingMode.Short,
            ReturnPolyline = false,
            Alternatives = 2,
            Avoid = RoutingAvoidFeature.Tolls | RoutingAvoidFeature.Ferries
        };

        Assert.That(request.Origin.Lat, Is.EqualTo(52.52));
        Assert.That(request.Destination.Lat, Is.EqualTo(48.8566));
        Assert.That(request.Via, Has.Count.EqualTo(1));
        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Pedestrian));
        Assert.That(request.RoutingMode, Is.EqualTo(RoutingMode.Short));
        Assert.That(request.ReturnPolyline, Is.False);
        Assert.That(request.Alternatives, Is.EqualTo(2));
        Assert.That(request.Avoid.HasFlag(RoutingAvoidFeature.Tolls), Is.True);
        Assert.That(request.Avoid.HasFlag(RoutingAvoidFeature.Ferries), Is.True);
        Assert.That(request.Avoid.HasFlag(RoutingAvoidFeature.Highways), Is.False);
    }
}
