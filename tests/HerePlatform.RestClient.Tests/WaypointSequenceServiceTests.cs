using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Routing;
using HerePlatform.Core.WaypointSequence;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class WaypointSequenceServiceTests
{
    private static RestWaypointSequenceService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestWaypointSequenceService(factory);
    }

    [Test]
    public async Task OptimizeSequenceAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"results":[]}""");
        var service = CreateService(handler);

        var request = new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.5, 13.4),
            End = new LatLngLiteral(48.1, 11.5),
            Waypoints = [new LatLngLiteral(50.0, 12.0), new LatLngLiteral(49.0, 11.0)],
            TransportMode = TransportMode.Car
        };

        await service.OptimizeSequenceAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://wps.hereapi.com/v8/findsequence2?"));
        Assert.That(url, Does.Contain("start=52.5%2C13.4"));
        Assert.That(url, Does.Contain("end=48.1%2C11.5"));
        Assert.That(url, Does.Contain("destination1=50%2C12"));
        Assert.That(url, Does.Contain("destination2=49%2C11"));
        Assert.That(url, Does.Contain("mode=fastest%3Bcar"));
    }

    [Test]
    public async Task OptimizeSequenceAsync_TruckMode_UsesCorrectModeParam()
    {
        var handler = MockHttpHandler.WithJson("""{"results":[]}""");
        var service = CreateService(handler);

        var request = new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.5, 13.4),
            End = new LatLngLiteral(48.1, 11.5),
            TransportMode = TransportMode.Truck
        };

        await service.OptimizeSequenceAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("mode=fastest%3Btruck"));
    }

    [Test]
    public async Task OptimizeSequenceAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "results": [{
                "waypoints": [
                    {"id": "start", "lat": 52.5, "lng": 13.4, "sequence": 0},
                    {"id": "destination2", "lat": 49.0, "lng": 11.0, "sequence": 1},
                    {"id": "destination1", "lat": 50.0, "lng": 12.0, "sequence": 2},
                    {"id": "end", "lat": 48.1, "lng": 11.5, "sequence": 3}
                ],
                "distance": 45000,
                "time": 3600
            }]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.5, 13.4),
            End = new LatLngLiteral(48.1, 11.5),
            Waypoints = [new LatLngLiteral(50.0, 12.0), new LatLngLiteral(49.0, 11.0)]
        };

        var result = await service.OptimizeSequenceAsync(request);

        Assert.That(result.TotalDistance, Is.EqualTo(45000));
        Assert.That(result.TotalDuration, Is.EqualTo(3600));
        // destination2 (index 1) comes first, destination1 (index 0) second
        Assert.That(result.OptimizedIndices, Is.EqualTo(new List<int> { 1, 0 }));
        Assert.That(result.OptimizedWaypoints, Has.Count.EqualTo(2));
        Assert.That(result.OptimizedWaypoints![0].Lat, Is.EqualTo(49.0));
        Assert.That(result.OptimizedWaypoints[1].Lat, Is.EqualTo(50.0));
    }

    [Test]
    public async Task OptimizeSequenceAsync_NoWaypoints_ReturnsEmptyResult()
    {
        var json = """
        {
            "results": [{
                "waypoints": [
                    {"id": "start", "lat": 52.5, "lng": 13.4, "sequence": 0},
                    {"id": "end", "lat": 48.1, "lng": 11.5, "sequence": 1}
                ],
                "distance": 500000,
                "time": 18000
            }]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.5, 13.4),
            End = new LatLngLiteral(48.1, 11.5)
        };

        var result = await service.OptimizeSequenceAsync(request);

        Assert.That(result.TotalDistance, Is.EqualTo(500000));
        Assert.That(result.TotalDuration, Is.EqualTo(18000));
        Assert.That(result.OptimizedIndices, Is.Empty);
        Assert.That(result.OptimizedWaypoints, Is.Empty);
    }

    [Test]
    public async Task OptimizeSequenceAsync_EmptyResponse_ReturnsDefaultResult()
    {
        var handler = MockHttpHandler.WithJson("""{"results":[]}""");
        var service = CreateService(handler);

        var request = new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.5, 13.4),
            End = new LatLngLiteral(48.1, 11.5)
        };

        var result = await service.OptimizeSequenceAsync(request);

        Assert.That(result.OptimizedIndices, Is.Null);
        Assert.That(result.OptimizedWaypoints, Is.Null);
        Assert.That(result.TotalDistance, Is.EqualTo(0));
        Assert.That(result.TotalDuration, Is.EqualTo(0));
    }

    [Test]
    public void OptimizeSequenceAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var request = new WaypointSequenceRequest
        {
            Start = new LatLngLiteral(52.5, 13.4),
            End = new LatLngLiteral(48.1, 11.5)
        };

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.OptimizeSequenceAsync(request));
        Assert.That(ex!.Service, Is.EqualTo("waypoint-sequence"));
    }
}
