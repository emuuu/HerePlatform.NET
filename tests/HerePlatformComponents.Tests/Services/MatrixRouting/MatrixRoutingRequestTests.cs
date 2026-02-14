using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.MatrixRouting;
using HerePlatformComponents.Maps.Services.Routing;

namespace HerePlatformComponents.Tests.Services.MatrixRouting;

[TestFixture]
public class MatrixRoutingRequestTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var request = new MatrixRoutingRequest();

        Assert.That(request.Origins, Is.Empty);
        Assert.That(request.Destinations, Is.Empty);
        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Car));
        Assert.That(request.RoutingMode, Is.EqualTo(RoutingMode.Fast));
        Assert.That(request.DepartureTime, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var origins = new List<LatLngLiteral>
        {
            new(52.52, 13.405),
            new(48.8566, 2.3522)
        };

        var destinations = new List<LatLngLiteral>
        {
            new(51.5074, -0.1278),
            new(40.4168, -3.7038)
        };

        var departureTime = new DateTime(2026, 6, 15, 8, 0, 0);

        var request = new MatrixRoutingRequest
        {
            Origins = origins,
            Destinations = destinations,
            TransportMode = TransportMode.Truck,
            RoutingMode = RoutingMode.Short,
            DepartureTime = departureTime
        };

        Assert.That(request.Origins, Has.Exactly(2).Items);
        Assert.That(request.Destinations, Has.Exactly(2).Items);
        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Truck));
        Assert.That(request.RoutingMode, Is.EqualTo(RoutingMode.Short));
        Assert.That(request.DepartureTime, Is.EqualTo(departureTime));
    }
}
