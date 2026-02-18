using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class PublicTransitServiceTests
{
    private static RestPublicTransitService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestPublicTransitService(factory);
    }

    // --- Departures ---

    [Test]
    public async Task GetDeparturesAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"boards":[]}""");
        var service = CreateService(handler);

        await service.GetDeparturesAsync(new LatLngLiteral(52.52, 13.37));

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://transit.hereapi.com/v8/departures?"));
        Assert.That(url, Does.Contain("in=52.52%2C13.37"));
    }

    [Test]
    public async Task GetDeparturesAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "boards": [
                {
                    "place": {"name": "Brandenburger Tor"},
                    "departures": [
                        {
                            "time": "2024-01-15T14:05:00+01:00",
                            "headsign": "S Wannsee",
                            "transport": {"name": "S1", "mode": "regionalTrain"}
                        },
                        {
                            "time": "2024-01-15T14:08:00+01:00",
                            "headsign": "Ahrensfelde",
                            "transport": {"name": "U5", "mode": "subway"}
                        }
                    ]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetDeparturesAsync(new LatLngLiteral(52.52, 13.37));

        Assert.That(result.Departures, Has.Count.EqualTo(2));
        Assert.That(result.Departures![0].LineName, Is.EqualTo("S1"));
        Assert.That(result.Departures[0].Direction, Is.EqualTo("S Wannsee"));
        Assert.That(result.Departures[0].DepartureTime, Is.EqualTo("2024-01-15T14:05:00+01:00"));
        Assert.That(result.Departures[0].TransportType, Is.EqualTo("regionalTrain"));
        Assert.That(result.Departures[0].StationName, Is.EqualTo("Brandenburger Tor"));
        Assert.That(result.Departures[1].LineName, Is.EqualTo("U5"));
        Assert.That(result.Departures[1].TransportType, Is.EqualTo("subway"));
    }

    [Test]
    public async Task GetDeparturesAsync_EmptyBoards_ReturnsEmpty()
    {
        var handler = MockHttpHandler.WithJson("""{"boards":[]}""");
        var service = CreateService(handler);

        var result = await service.GetDeparturesAsync(new LatLngLiteral(52.52, 13.37));

        Assert.That(result.Departures, Is.Not.Null);
        Assert.That(result.Departures, Is.Empty);
    }

    [Test]
    public void GetDeparturesAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.GetDeparturesAsync(new LatLngLiteral(52.52, 13.37)));
        Assert.That(ex!.Service, Is.EqualTo("transit"));
    }

    // --- Stations ---

    [Test]
    public async Task SearchStationsAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"stations":[]}""");
        var service = CreateService(handler);

        await service.SearchStationsAsync(new LatLngLiteral(52.52, 13.37), 1000);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://transit.hereapi.com/v8/stations?"));
        Assert.That(url, Does.Contain("in=52.52%2C13.37%3Br%3D1000"));
    }

    [Test]
    public async Task SearchStationsAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "stations": [
                {
                    "place": {
                        "name": "Brandenburger Tor",
                        "location": {"lat": 52.5163, "lng": 13.3777}
                    },
                    "distance": 120,
                    "transports": [
                        {"name": "S1", "mode": "regionalTrain"},
                        {"name": "U5", "mode": "subway"},
                        {"name": "S2", "mode": "regionalTrain"}
                    ]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SearchStationsAsync(new LatLngLiteral(52.52, 13.37));

        Assert.That(result.Stations, Has.Count.EqualTo(1));
        var station = result.Stations![0];
        Assert.That(station.Name, Is.EqualTo("Brandenburger Tor"));
        Assert.That(station.Position!.Value.Lat, Is.EqualTo(52.5163));
        Assert.That(station.Distance, Is.EqualTo(120));
        Assert.That(station.TransportTypes, Has.Count.EqualTo(2)); // distinct modes
        Assert.That(station.TransportTypes, Does.Contain("regionalTrain"));
        Assert.That(station.TransportTypes, Does.Contain("subway"));
    }

    [Test]
    public async Task SearchStationsAsync_EmptyResponse_ReturnsEmpty()
    {
        var handler = MockHttpHandler.WithJson("""{"stations":[]}""");
        var service = CreateService(handler);

        var result = await service.SearchStationsAsync(new LatLngLiteral(52.52, 13.37));

        Assert.That(result.Stations, Is.Not.Null);
        Assert.That(result.Stations, Is.Empty);
    }

    [Test]
    public void SearchStationsAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.SearchStationsAsync(new LatLngLiteral(52.52, 13.37)));
        Assert.That(ex!.Service, Is.EqualTo("transit"));
    }

    [Test]
    public void GetDeparturesAsync_400_ThrowsHereApiException()
    {
        var handler = MockHttpHandler.WithJson("""{"error":"Bad request"}""", HttpStatusCode.BadRequest);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiException>(
            () => service.GetDeparturesAsync(new LatLngLiteral(52.52, 13.37)));
        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Service, Is.EqualTo("transit"));
    }

    [Test]
    public void SearchStationsAsync_400_ThrowsHereApiException()
    {
        var handler = MockHttpHandler.WithJson("""{"error":"Invalid radius"}""", HttpStatusCode.BadRequest);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiException>(
            () => service.SearchStationsAsync(new LatLngLiteral(52.52, 13.37)));
        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Service, Is.EqualTo("transit"));
    }
}
