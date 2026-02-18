using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.TourPlanning;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class TourPlanningServiceTests
{
    private static RestTourPlanningService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestTourPlanningService(factory);
    }

    private static TourPlanningProblem CreateSimpleProblem()
    {
        return new TourPlanningProblem
        {
            Plan = new TourPlan
            {
                Jobs =
                [
                    new TourJob
                    {
                        Id = "job1",
                        Places = new TourJobPlaces
                        {
                            Deliveries =
                            [
                                new TourJobPlace
                                {
                                    Location = new LatLngLiteral(52.5, 13.4),
                                    Duration = 300
                                }
                            ]
                        }
                    }
                ]
            },
            Fleet = new TourFleet
            {
                Types =
                [
                    new TourVehicleType
                    {
                        Id = "vehicle1",
                        Profile = "car",
                        Costs = new TourVehicleCosts { Fixed = 10, Distance = 0.001, Time = 0.01 },
                        Shifts =
                        [
                            new TourVehicleShift
                            {
                                Start = new TourShiftEnd
                                {
                                    Location = new LatLngLiteral(52.53, 13.38),
                                    Time = "2024-01-15T08:00:00Z"
                                },
                                End = new TourShiftEnd
                                {
                                    Location = new LatLngLiteral(52.53, 13.38),
                                    Time = "2024-01-15T18:00:00Z"
                                }
                            }
                        ],
                        Capacity = [10],
                        Amount = 1
                    }
                ]
            }
        };
    }

    [Test]
    public async Task SolveAsync_SendsPostRequest()
    {
        var json = """{"tours":[],"statistic":{}}""";
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        await service.SolveAsync(CreateSimpleProblem());

        Assert.That(handler.LastRequest!.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(handler.LastRequest.RequestUri!.ToString(),
            Does.StartWith("https://tourplanning.hereapi.com/v3/problems"));
    }

    [Test]
    public async Task SolveAsync_SendsCorrectBody()
    {
        var json = """{"tours":[],"statistic":{}}""";
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        await service.SolveAsync(CreateSimpleProblem());

        var body = handler.LastRequestBody;
        Assert.That(body, Does.Contain("\"id\":\"job1\""));
        Assert.That(body, Does.Contain("\"profile\":\"car\""));
        Assert.That(body, Does.Contain("\"lat\":52.5"));
        Assert.That(body, Does.Contain("\"lng\":13.4"));
        Assert.That(body, Does.Contain("\"duration\":300"));
    }

    [Test]
    public async Task SolveAsync_MapsTourResponseCorrectly()
    {
        var json = """
        {
            "tours": [
                {
                    "vehicleId": "vehicle1",
                    "stops": [
                        {
                            "location": {"lat": 52.53, "lng": 13.38},
                            "activities": [{"type": "departure"}]
                        },
                        {
                            "location": {"lat": 52.5, "lng": 13.4},
                            "activities": [{"type": "delivery", "jobId": "job1"}]
                        },
                        {
                            "location": {"lat": 52.53, "lng": 13.38},
                            "activities": [{"type": "arrival"}]
                        }
                    ],
                    "statistic": {"cost": 25.5, "distance": 5000, "duration": 1800}
                }
            ],
            "statistic": {"cost": 25.5, "distance": 5000, "duration": 1800}
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SolveAsync(CreateSimpleProblem());

        Assert.That(result.Tours, Has.Count.EqualTo(1));
        var tour = result.Tours![0];
        Assert.That(tour.VehicleId, Is.EqualTo("vehicle1"));
        Assert.That(tour.Stops, Has.Count.EqualTo(3));
        Assert.That(tour.Stops![0].Location!.Value.Lat, Is.EqualTo(52.53));
        Assert.That(tour.Stops[0].Activities![0].Type, Is.EqualTo("departure"));
        Assert.That(tour.Stops[1].Activities![0].Type, Is.EqualTo("delivery"));
        Assert.That(tour.Stops[1].Activities![0].JobId, Is.EqualTo("job1"));
        Assert.That(tour.Statistic!.Cost, Is.EqualTo(25.5));
        Assert.That(tour.Statistic.Distance, Is.EqualTo(5000));
        Assert.That(tour.Statistic.Duration, Is.EqualTo(1800));
    }

    [Test]
    public async Task SolveAsync_MapsUnassignedJobs()
    {
        var json = """
        {
            "tours": [],
            "unassignedJobs": ["job2", "job3"],
            "statistic": {"cost": 0, "distance": 0, "duration": 0}
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SolveAsync(CreateSimpleProblem());

        Assert.That(result.UnassignedJobs, Has.Count.EqualTo(2));
        Assert.That(result.UnassignedJobs![0], Is.EqualTo("job2"));
        Assert.That(result.UnassignedJobs[1], Is.EqualTo("job3"));
    }

    [Test]
    public async Task SolveAsync_MapsOverallStatistics()
    {
        var json = """
        {
            "tours": [],
            "statistic": {"cost": 100.5, "distance": 25000, "duration": 7200}
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SolveAsync(CreateSimpleProblem());

        Assert.That(result.Statistic, Is.Not.Null);
        Assert.That(result.Statistic!.Cost, Is.EqualTo(100.5));
        Assert.That(result.Statistic.Distance, Is.EqualTo(25000));
        Assert.That(result.Statistic.Duration, Is.EqualTo(7200));
    }

    [Test]
    public async Task SolveAsync_EmptyResponse_ReturnsEmptyResult()
    {
        var handler = MockHttpHandler.WithJson("""{}""");
        var service = CreateService(handler);

        var result = await service.SolveAsync(CreateSimpleProblem());

        Assert.That(result.Tours, Is.Empty);
    }

    [Test]
    public void SolveAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.SolveAsync(CreateSimpleProblem()));
        Assert.That(ex!.Service, Is.EqualTo("tourPlanning"));
    }
}
