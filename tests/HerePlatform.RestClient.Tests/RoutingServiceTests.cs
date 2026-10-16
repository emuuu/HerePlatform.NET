using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Routing;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class RoutingServiceTests
{
    private static RestRoutingService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestRoutingService(factory);
    }

    [Test]
    public async Task CalculateRouteAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            TransportMode = TransportMode.Car,
            RoutingMode = RoutingMode.Fast
        };

        await service.CalculateRouteAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://router.hereapi.com/v8/routes?"));
        Assert.That(url, Does.Contain("origin=52.5%2C13.4"));
        Assert.That(url, Does.Contain("destination=48.1%2C11.5"));
        Assert.That(url, Does.Contain("transportMode=car"));
        Assert.That(url, Does.Contain("routingMode=fast"));
        Assert.That(url, Does.Contain("return=summary%2Cpolyline"));
    }

    [Test]
    public async Task CalculateRouteAsync_WithInstructions_IncludesTurnByTurnActions()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            ReturnInstructions = true
        };

        await service.CalculateRouteAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("turnByTurnActions"));
    }

    [Test]
    public async Task CalculateRouteAsync_WithViaPoints()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            Via = [new LatLngLiteral(50.0, 12.0), new LatLngLiteral(49.0, 11.0)]
        };

        await service.CalculateRouteAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("via=50%2C12"));
        Assert.That(url, Does.Contain("via=49%2C11"));
    }

    [Test]
    public async Task CalculateRouteAsync_WithAvoidFeatures()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            Avoid = RoutingAvoidFeature.Tolls | RoutingAvoidFeature.Ferries
        };

        await service.CalculateRouteAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("avoid%5Bfeatures%5D=tollRoad%2Cferry"));
    }

    [Test]
    public async Task CalculateRouteAsync_MapsResponseWithPolyline()
    {
        // "BFoz5xJ67i1B" is a valid flexible polyline encoding for a short segment
        var json = """
        {
            "routes": [{
                "sections": [{
                    "polyline": "BFoz5xJ67i1B1B7E7E",
                    "summary": {"duration": 3600, "length": 50000, "baseDuration": 3400},
                    "transport": {"mode": "car"}
                }]
            }]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5)
        };

        var result = await service.CalculateRouteAsync(request);

        Assert.That(result.Routes, Has.Count.EqualTo(1));
        var section = result.Routes![0].Sections![0];
        Assert.That(section.Summary, Is.Not.Null);
        Assert.That(section.Summary!.Duration, Is.EqualTo(3600));
        Assert.That(section.Summary.Length, Is.EqualTo(50000));
        Assert.That(section.Summary.BaseDuration, Is.EqualTo(3400));
        Assert.That(section.Transport, Is.EqualTo("car"));
        Assert.That(section.Polyline, Is.EqualTo("BFoz5xJ67i1B1B7E7E"));
        // DecodedPolyline should be populated (even if short)
        Assert.That(section.DecodedPolyline, Is.Not.Null);
        Assert.That(section.DecodedPolyline, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task CalculateRouteAsync_WithTurnActions()
    {
        var json = """
        {
            "routes": [{
                "sections": [{
                    "polyline": "BFoz5xJ67i1B1B7E7E",
                    "summary": {"duration": 100, "length": 1000},
                    "transport": {"mode": "car"},
                    "turnByTurnActions": [
                        {"action": "depart", "instruction": "Head north", "duration": 30, "length": 500, "offset": 0},
                        {"action": "arrive", "instruction": "Arrive", "duration": 0, "length": 0, "offset": 5}
                    ]
                }]
            }]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            ReturnInstructions = true
        };

        var result = await service.CalculateRouteAsync(request);

        var actions = result.Routes![0].Sections![0].TurnByTurnActions;
        Assert.That(actions, Has.Count.EqualTo(2));
        Assert.That(actions![0].Action, Is.EqualTo("depart"));
        Assert.That(actions[0].Instruction, Is.EqualTo("Head north"));
        Assert.That(actions[1].Action, Is.EqualTo("arrive"));
    }

    [Test]
    public async Task CalculateRouteAsync_EmptyResponse_ReturnsEmptyRoutes()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5)
        };

        var result = await service.CalculateRouteAsync(request);

        Assert.That(result.Routes, Is.Not.Null);
        Assert.That(result.Routes, Is.Empty);
    }

    [Test]
    public async Task CalculateRouteAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5)
        };

        var ex = await Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.CalculateRouteAsync(request));
        Assert.That(ex!.Service, Is.EqualTo("routing"));
    }

    [Test]
    public async Task CalculateRouteAsync_TruckOptions_AddsParams()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            TransportMode = TransportMode.Truck,
            Truck = new TruckOptions
            {
                Height = 4.0,
                Width = 2.5,
                GrossWeight = 40000
            }
        };

        await service.CalculateRouteAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("truck%5Bheight%5D=400"));
        Assert.That(url, Does.Contain("truck%5Bwidth%5D=250"));
        Assert.That(url, Does.Contain("truck%5BgrossWeight%5D=40000"));
    }

    [Test]
    public async Task CalculateRouteAsync_NoPolyline_DoesNotRequestPolyline()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            ReturnPolyline = false
        };

        await service.CalculateRouteAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("return=summary"));
        Assert.That(url, Does.Not.Contain("polyline"));
    }
}
