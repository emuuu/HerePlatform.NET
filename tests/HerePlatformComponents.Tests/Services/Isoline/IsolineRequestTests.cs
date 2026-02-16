using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.Isoline;

namespace HerePlatformComponents.Tests.Services.Isoline;

[TestFixture]
public class IsolineRequestTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var request = new IsolineRequest();

        Assert.That(request.RangeType, Is.EqualTo(IsolineRangeType.Time));
        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Car));
        Assert.That(request.RoutingMode, Is.EqualTo(RoutingMode.Fast));
        Assert.That(request.Avoid, Is.EqualTo(RoutingAvoidFeature.None));
        Assert.That(request.Ranges, Is.Null);
        Assert.That(request.DepartureTime, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var request = new IsolineRequest
        {
            Center = new LatLngLiteral(52.52, 13.405),
            Ranges = new List<int> { 300, 600, 900 },
            RangeType = IsolineRangeType.Distance,
            TransportMode = TransportMode.Pedestrian,
            RoutingMode = RoutingMode.Short,
            Avoid = RoutingAvoidFeature.Tolls,
            DepartureTime = "2024-01-15T08:00:00"
        };

        Assert.That(request.Center.Lat, Is.EqualTo(52.52));
        Assert.That(request.Ranges, Has.Count.EqualTo(3));
        Assert.That(request.RangeType, Is.EqualTo(IsolineRangeType.Distance));
        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Pedestrian));
        Assert.That(request.RoutingMode, Is.EqualTo(RoutingMode.Short));
        Assert.That(request.Avoid, Is.EqualTo(RoutingAvoidFeature.Tolls));
        Assert.That(request.DepartureTime, Is.EqualTo("2024-01-15T08:00:00"));
    }
}
