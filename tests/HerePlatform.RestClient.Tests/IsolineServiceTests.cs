using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Isoline;
using HerePlatform.Core.Routing;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class IsolineServiceTests
{
    private static RestIsolineService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestIsolineService(factory);
    }

    [Test]
    public async Task CalculateIsolineAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"isolines":[]}""");
        var service = CreateService(handler);

        var request = new IsolineRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Ranges = [300, 600],
            RangeType = IsolineRangeType.Time,
            TransportMode = TransportMode.Car
        };

        await service.CalculateIsolineAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://isoline.router.hereapi.com/v8/isolines?"));
        Assert.That(url, Does.Contain("origin=52.5%2C13.4"));
        Assert.That(url, Does.Contain("range%5Bvalues%5D=300%2C600"));
        Assert.That(url, Does.Contain("range%5Btype%5D=time"));
        Assert.That(url, Does.Contain("transportMode=car"));
    }

    [Test]
    public async Task CalculateIsolineAsync_MapsResponseWithPolylineDecode()
    {
        // Use a simple valid flexible polyline
        var json = """
        {
            "isolines": [
                {
                    "range": {"value": 300},
                    "polygons": [{"outer": "BFoz5xJ67i1B1B7E7E"}]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new IsolineRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Ranges = [300]
        };

        var result = await service.CalculateIsolineAsync(request);

        Assert.That(result.Isolines, Has.Count.EqualTo(1));
        Assert.That(result.Isolines![0].Range, Is.EqualTo(300));
        Assert.That(result.Isolines[0].EncodedPolyline, Is.EqualTo("BFoz5xJ67i1B1B7E7E"));
        Assert.That(result.Isolines[0].Polygon, Is.Not.Null);
        Assert.That(result.Isolines[0].Polygon, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task CalculateIsolineAsync_EmptyResponse_ReturnsEmptyIsolines()
    {
        var handler = MockHttpHandler.WithJson("""{"isolines":[]}""");
        var service = CreateService(handler);

        var request = new IsolineRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Ranges = [300]
        };

        var result = await service.CalculateIsolineAsync(request);

        Assert.That(result.Isolines, Is.Not.Null);
        Assert.That(result.Isolines, Is.Empty);
    }

    [Test]
    public async Task CalculateIsolineAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var request = new IsolineRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Ranges = [300]
        };

        var ex = await Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.CalculateIsolineAsync(request));
        Assert.That(ex!.Service, Is.EqualTo("isoline"));
    }

    [Test]
    public async Task CalculateIsolineAsync_WithAvoidFeatures()
    {
        var handler = MockHttpHandler.WithJson("""{"isolines":[]}""");
        var service = CreateService(handler);

        var request = new IsolineRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Ranges = [300],
            Avoid = RoutingAvoidFeature.Tolls | RoutingAvoidFeature.Highways
        };

        await service.CalculateIsolineAsync(request);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("avoid%5Bfeatures%5D=tollRoad%2CcontrolledAccessHighway"));
    }
}
