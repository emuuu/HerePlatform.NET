using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.IntermodalRouting;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class IntermodalRoutingServiceTests
{
    private static RestIntermodalRoutingService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestIntermodalRoutingService(factory);
    }

    [Test]
    public async Task CalculateRouteAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        await service.CalculateRouteAsync(new IntermodalRoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            Lang = "de"
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://intermodal.router.hereapi.com/v8/routes?"));
        Assert.That(url, Does.Contain("origin=52.5%2C13.4"));
        Assert.That(url, Does.Contain("destination=48.1%2C11.5"));
        Assert.That(url, Does.Contain("lang=de"));
        Assert.That(url, Does.Contain("return=polyline%2CtravelSummary"));
    }

    [Test]
    public async Task CalculateRouteAsync_IncludesDepartAt()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        await service.CalculateRouteAsync(new IntermodalRoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            DepartAt = "2024-01-15T08:00:00"
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("departAt=2024-01-15T08%3A00%3A00"));
    }

    [Test]
    public async Task CalculateRouteAsync_IncludesArriveAt()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        await service.CalculateRouteAsync(new IntermodalRoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5),
            ArriveAt = "2024-01-15T09:00:00"
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("arriveAt=2024-01-15T09%3A00%3A00"));
    }

    [Test]
    public async Task CalculateRouteAsync_MapsMultiSectionRoute()
    {
        var json = """
        {
            "routes": [
                {
                    "sections": [
                        {
                            "type": "pedestrian",
                            "departure": {
                                "name": "Start",
                                "location": {"lat": 52.5, "lng": 13.4},
                                "time": "2024-01-15T08:00:00"
                            },
                            "arrival": {
                                "name": "U Friedrichstr.",
                                "location": {"lat": 52.52, "lng": 13.39},
                                "time": "2024-01-15T08:10:00"
                            },
                            "travelSummary": {"duration": 600, "length": 800},
                            "transport": {"mode": "pedestrian"}
                        },
                        {
                            "type": "transit",
                            "departure": {
                                "name": "U Friedrichstr.",
                                "location": {"lat": 52.52, "lng": 13.39},
                                "time": "2024-01-15T08:12:00"
                            },
                            "arrival": {
                                "name": "U Hauptbahnhof",
                                "location": {"lat": 52.53, "lng": 13.37},
                                "time": "2024-01-15T08:18:00"
                            },
                            "travelSummary": {"duration": 360, "length": 2000},
                            "transport": {
                                "mode": "subway",
                                "name": "U6",
                                "headsign": "Alt-Tegel",
                                "shortName": "U6",
                                "color": "#7B3F98"
                            }
                        },
                        {
                            "type": "pedestrian",
                            "departure": {
                                "name": "U Hauptbahnhof",
                                "location": {"lat": 52.53, "lng": 13.37},
                                "time": "2024-01-15T08:18:00"
                            },
                            "arrival": {
                                "name": "Destination",
                                "location": {"lat": 52.54, "lng": 13.36},
                                "time": "2024-01-15T08:25:00"
                            },
                            "travelSummary": {"duration": 420, "length": 500},
                            "transport": {"mode": "pedestrian"}
                        }
                    ]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.CalculateRouteAsync(new IntermodalRoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(52.54, 13.36)
        });

        Assert.That(result.Routes, Has.Count.EqualTo(1));
        var route = result.Routes![0];
        Assert.That(route.Sections, Has.Count.EqualTo(3));

        // Pedestrian section
        var walk = route.Sections![0];
        Assert.That(walk.Type, Is.EqualTo("pedestrian"));
        Assert.That(walk.Departure!.Name, Is.EqualTo("Start"));
        Assert.That(walk.Departure.Position!.Value.Lat, Is.EqualTo(52.5));
        Assert.That(walk.Summary!.Duration, Is.EqualTo(600));
        Assert.That(walk.Summary.Length, Is.EqualTo(800));

        // Transit section
        var transit = route.Sections[1];
        Assert.That(transit.Type, Is.EqualTo("transit"));
        Assert.That(transit.Transport!.Mode, Is.EqualTo("subway"));
        Assert.That(transit.Transport.Name, Is.EqualTo("U6"));
        Assert.That(transit.Transport.Headsign, Is.EqualTo("Alt-Tegel"));
        Assert.That(transit.Transport.ShortName, Is.EqualTo("U6"));
        Assert.That(transit.Transport.Color, Is.EqualTo("#7B3F98"));
    }

    [Test]
    public async Task CalculateRouteAsync_DecodesPolyline()
    {
        // "BFoz5xJ67i1B1B7PB" is a valid flexible polyline encoding for a short segment
        var json = """
        {
            "routes": [
                {
                    "sections": [
                        {
                            "type": "pedestrian",
                            "polyline": "BFoz5xJ67i1B1B7PB"
                        }
                    ]
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.CalculateRouteAsync(new IntermodalRoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(52.51, 13.41)
        });

        var section = result.Routes![0].Sections![0];
        Assert.That(section.Polyline, Is.Not.Null);
        Assert.That(section.Geometry, Is.Not.Null);
        Assert.That(section.Geometry, Is.Not.Empty);
    }

    [Test]
    public async Task CalculateRouteAsync_EmptyResponse_ReturnsEmptyRoutes()
    {
        var handler = MockHttpHandler.WithJson("""{"routes":[]}""");
        var service = CreateService(handler);

        var result = await service.CalculateRouteAsync(new IntermodalRoutingRequest
        {
            Origin = new LatLngLiteral(52.5, 13.4),
            Destination = new LatLngLiteral(48.1, 11.5)
        });

        Assert.That(result.Routes, Is.Not.Null);
        Assert.That(result.Routes, Is.Empty);
    }

    [Test]
    public void CalculateRouteAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.CalculateRouteAsync(new IntermodalRoutingRequest
            {
                Origin = new LatLngLiteral(52.5, 13.4),
                Destination = new LatLngLiteral(48.1, 11.5)
            }));
        Assert.That(ex!.Service, Is.EqualTo("intermodalRouting"));
    }
}
