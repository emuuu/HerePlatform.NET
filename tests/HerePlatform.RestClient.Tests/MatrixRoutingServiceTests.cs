using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.MatrixRouting;
using HerePlatform.Core.Routing;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class MatrixRoutingServiceTests
{
    private static RestMatrixRoutingService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestMatrixRoutingService(factory);
    }

    [Test]
    public async Task CalculateMatrixAsync_SendsPostRequest()
    {
        var json = """
        {
            "matrix": {
                "numOrigins": 2,
                "numDestinations": 2,
                "entries": []
            }
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new MatrixRoutingRequest
        {
            Origins = [new LatLngLiteral(52.5, 13.4), new LatLngLiteral(48.1, 11.5)],
            Destinations = [new LatLngLiteral(50.0, 12.0), new LatLngLiteral(51.0, 13.0)]
        };

        await service.CalculateMatrixAsync(request);

        Assert.That(handler.LastRequest!.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(handler.LastRequest.RequestUri!.ToString(),
            Does.StartWith("https://matrix.router.hereapi.com/v8/matrix"));
    }

    [Test]
    public async Task CalculateMatrixAsync_SendsCorrectBody()
    {
        var json = """
        {
            "matrix": {
                "numOrigins": 1,
                "numDestinations": 1,
                "entries": []
            }
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new MatrixRoutingRequest
        {
            Origins = [new LatLngLiteral(52.5, 13.4)],
            Destinations = [new LatLngLiteral(48.1, 11.5)],
            TransportMode = TransportMode.Truck
        };

        await service.CalculateMatrixAsync(request);

        var body = handler.LastRequestBody;
        Assert.That(body, Does.Contain("\"lat\":52.5"));
        Assert.That(body, Does.Contain("\"lng\":13.4"));
        Assert.That(body, Does.Contain("\"profile\":\"truck\""));
    }

    [Test]
    public async Task CalculateMatrixAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "matrix": {
                "numOrigins": 2,
                "numDestinations": 2,
                "entries": [
                    {"originIndex": 0, "destinationIndex": 0, "travelTime": 0, "distance": 0},
                    {"originIndex": 0, "destinationIndex": 1, "travelTime": 3600, "distance": 50000},
                    {"originIndex": 1, "destinationIndex": 0, "travelTime": 3700, "distance": 51000},
                    {"originIndex": 1, "destinationIndex": 1, "travelTime": 0, "distance": 0}
                ]
            }
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var request = new MatrixRoutingRequest
        {
            Origins = [new LatLngLiteral(52.5, 13.4), new LatLngLiteral(48.1, 11.5)],
            Destinations = [new LatLngLiteral(50.0, 12.0), new LatLngLiteral(51.0, 13.0)]
        };

        var result = await service.CalculateMatrixAsync(request);

        Assert.That(result.NumOrigins, Is.EqualTo(2));
        Assert.That(result.NumDestinations, Is.EqualTo(2));
        Assert.That(result.Matrix, Has.Count.EqualTo(4));
        Assert.That(result.Matrix[1].OriginIndex, Is.EqualTo(0));
        Assert.That(result.Matrix[1].DestinationIndex, Is.EqualTo(1));
        Assert.That(result.Matrix[1].Duration, Is.EqualTo(3600));
        Assert.That(result.Matrix[1].Length, Is.EqualTo(50000));
    }

    [Test]
    public void CalculateMatrixAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var request = new MatrixRoutingRequest
        {
            Origins = [new LatLngLiteral(52.5, 13.4)],
            Destinations = [new LatLngLiteral(48.1, 11.5)]
        };

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.CalculateMatrixAsync(request));
        Assert.That(ex!.Service, Is.EqualTo("matrix"));
    }

    [Test]
    public async Task CalculateMatrixAsync_NullMatrix_ReturnsEmpty()
    {
        var handler = MockHttpHandler.WithJson("""{}""");
        var service = CreateService(handler);

        var request = new MatrixRoutingRequest
        {
            Origins = [new LatLngLiteral(52.5, 13.4)],
            Destinations = [new LatLngLiteral(48.1, 11.5)]
        };

        var result = await service.CalculateMatrixAsync(request);

        Assert.That(result.Matrix, Is.Empty);
    }
}
