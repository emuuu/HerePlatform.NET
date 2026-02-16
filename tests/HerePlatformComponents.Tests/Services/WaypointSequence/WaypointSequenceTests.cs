using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.WaypointSequence;

namespace HerePlatformComponents.Tests.Services.WaypointSequence;

[TestFixture]
public class WaypointSequenceTests
{
    [Test]
    public void WaypointSequenceRequest_DefaultValues()
    {
        var request = new WaypointSequenceRequest();

        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Car));
        Assert.That(request.Waypoints, Is.Null);
    }

    [Test]
    public void WaypointSequenceRequest_WithValues()
    {
        var request = new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.52, 13.405),
            End = new LatLngLiteral(48.8566, 2.3522),
            TransportMode = TransportMode.Truck,
            Waypoints = new List<LatLngLiteral>
            {
                new(50.0, 8.0),
                new(49.0, 6.0)
            }
        };

        Assert.That(request.Start.Lat, Is.EqualTo(52.52));
        Assert.That(request.End.Lat, Is.EqualTo(48.8566));
        Assert.That(request.TransportMode, Is.EqualTo(TransportMode.Truck));
        Assert.That(request.Waypoints, Has.Count.EqualTo(2));
    }

    [Test]
    public void WaypointSequenceResult_DefaultValues()
    {
        var result = new WaypointSequenceResult();

        Assert.That(result.OptimizedIndices, Is.Null);
        Assert.That(result.OptimizedWaypoints, Is.Null);
        Assert.That(result.TotalDistance, Is.EqualTo(0));
        Assert.That(result.TotalDuration, Is.EqualTo(0));
    }

    [Test]
    public void WaypointSequenceResult_WithValues()
    {
        var result = new WaypointSequenceResult
        {
            OptimizedIndices = new List<int> { 1, 0, 2 },
            OptimizedWaypoints = new List<LatLngLiteral>
            {
                new(49.0, 6.0),
                new(50.0, 8.0),
                new(47.0, 7.0)
            },
            TotalDistance = 150000,
            TotalDuration = 7200
        };

        Assert.That(result.OptimizedIndices, Has.Count.EqualTo(3));
        Assert.That(result.OptimizedIndices![0], Is.EqualTo(1));
        Assert.That(result.TotalDistance, Is.EqualTo(150000));
        Assert.That(result.TotalDuration, Is.EqualTo(7200));
    }
}
