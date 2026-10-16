using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.RouteMatching;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Utilities;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class RouteMatchingServiceTests
{
    private static RestRouteMatchingService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestRouteMatchingService(factory);
    }

    [Test]
    public async Task MatchRouteAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"routeLinks":[]}""");
        var service = CreateService(handler);

        await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TransportMode = TransportMode.Car,
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch }
            ]
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://routematching.hereapi.com/v8/match/routelinks?"));
        Assert.That(url, Does.Contain("mode=fastest%3Bcar"));
        Assert.That(url, Does.Contain("routeMatch=1"));
    }

    [Test]
    public async Task MatchRouteAsync_TruckMode_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"routeLinks":[]}""");
        var service = CreateService(handler);

        await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TransportMode = TransportMode.Truck,
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch }
            ]
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("mode=fastest%3Btruck"));
    }

    [Test]
    public async Task MatchRouteAsync_UsesPostMethod()
    {
        var handler = MockHttpHandler.WithJson("""{"routeLinks":[]}""");
        var service = CreateService(handler);

        await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch }
            ]
        });

        Assert.That(handler.LastRequest!.Method, Is.EqualTo(HttpMethod.Post));
    }

    [Test]
    public async Task MatchRouteAsync_SendsTracePointsInBody()
    {
        string? capturedBody = null;
        var handler = new MockHttpHandler(req =>
        {
            capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"routeLinks":[]}""", System.Text.Encoding.UTF8, "application/json")
            };
        });
        var service = CreateService(handler);

        var ts = new DateTimeOffset(2025, 1, 15, 12, 0, 0, TimeSpan.Zero);
        await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = ts, Heading = 90.0, Speed = 15.5 }
            ]
        });

        Assert.That(capturedBody, Does.Contain("52.5"));
        Assert.That(capturedBody, Does.Contain("13.4"));
        Assert.That(capturedBody, Does.Contain("90"));
        Assert.That(capturedBody, Does.Contain("15.5"));
    }

    [Test]
    public async Task MatchRouteAsync_MultipleTracePoints_SeparatedByNewline()
    {
        string? capturedBody = null;
        var handler = new MockHttpHandler(req =>
        {
            capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"routeLinks":[]}""", System.Text.Encoding.UTF8, "application/json")
            };
        });
        var service = CreateService(handler);

        await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch },
                new TracePoint { Position = new LatLngLiteral(52.51, 13.41), Timestamp = DateTimeOffset.UnixEpoch.AddSeconds(10) }
            ]
        });

        var lines = capturedBody!.Split('\n');
        Assert.That(lines, Has.Length.EqualTo(2));
        Assert.That(lines[0], Does.StartWith("52.5,13.4,"));
        Assert.That(lines[1], Does.StartWith("52.51,13.41,"));
    }

    [Test]
    public async Task MatchRouteAsync_MapsResponseWithFlexiblePolyline()
    {
        // Create a flexible polyline for two points
        var points = new List<LatLngLiteral>
        {
            new(52.5, 13.4),
            new(52.51, 13.41)
        };
        var polyline = FlexiblePolyline.Encode(points);

        var json = $$"""
        {
            "routeLinks": [
                {
                    "linkId": "here:cm:segment:123456789",
                    "confidence": 0.95,
                    "speedLimit": 50.0,
                    "functionalClass": 2,
                    "shape": "{{polyline}}"
                }
            ],
            "warnings": ["Some trace points were not matched"]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch }
            ]
        });

        Assert.That(result.MatchedLinks, Has.Count.EqualTo(1));
        var link = result.MatchedLinks![0];
        Assert.That(link.LinkId, Is.EqualTo("here:cm:segment:123456789"));
        Assert.That(link.Confidence, Is.EqualTo(0.95));
        Assert.That(link.SpeedLimit, Is.EqualTo(50.0));
        Assert.That(link.FunctionalClass, Is.EqualTo(2));
        Assert.That(link.Geometry, Has.Count.EqualTo(2));
        Assert.That(link.Geometry![0].Lat, Is.EqualTo(52.5).Within(0.0001));
        Assert.That(link.Geometry[0].Lng, Is.EqualTo(13.4).Within(0.0001));
        Assert.That(link.Geometry[1].Lat, Is.EqualTo(52.51).Within(0.0001));
        Assert.That(link.Geometry[1].Lng, Is.EqualTo(13.41).Within(0.0001));

        Assert.That(result.Warnings, Has.Count.EqualTo(1));
        Assert.That(result.Warnings![0], Does.Contain("not matched"));
    }

    [Test]
    public async Task MatchRouteAsync_NullShape_MapsToNullGeometry()
    {
        var json = """
        {
            "routeLinks": [
                {
                    "linkId": "here:cm:segment:123456789",
                    "confidence": 0.8
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch }
            ]
        });

        Assert.That(result.MatchedLinks![0].Geometry, Is.Null);
    }

    [Test]
    public async Task MatchRouteAsync_EmptyTracePoints_SendsEmptyBody()
    {
        string? capturedBody = null;
        var handler = new MockHttpHandler(req =>
        {
            capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"routeLinks":[]}""", System.Text.Encoding.UTF8, "application/json")
            };
        });
        var service = CreateService(handler);

        await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TracePoints = []
        });

        Assert.That(capturedBody, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task MatchRouteAsync_EmptyResponse_ReturnsEmptyLinks()
    {
        var handler = MockHttpHandler.WithJson("""{"routeLinks":[]}""");
        var service = CreateService(handler);

        var result = await service.MatchRouteAsync(new RouteMatchingRequest
        {
            TracePoints =
            [
                new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch }
            ]
        });

        Assert.That(result.MatchedLinks, Is.Not.Null);
        Assert.That(result.MatchedLinks, Is.Empty);
    }

    [Test]
    public async Task MatchRouteAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = await Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.MatchRouteAsync(new RouteMatchingRequest
            {
                TracePoints =
                [
                    new TracePoint { Position = new LatLngLiteral(52.5, 13.4), Timestamp = DateTimeOffset.UnixEpoch }
                ]
            }));
        Assert.That(ex!.Service, Is.EqualTo("route-matching"));
    }
}
