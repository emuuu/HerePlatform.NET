using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services;
using HerePlatformComponents.Maps.Services.Traffic;

namespace HerePlatformComponents.Tests.Services.Traffic;

[TestFixture]
public class TrafficServiceTests : ServiceTestBase
{
    [Test]
    public async Task GetTrafficIncidentsAsync_WithIncidents_ReturnsTypesAndSeverities()
    {
        MockJsResult("blazorHerePlatform.objectManager.getTrafficIncidents", new TrafficIncidentsResult
        {
            Incidents = new List<TrafficIncident>
            {
                new()
                {
                    Type = "accident",
                    Severity = 3,
                    Description = "Multi-vehicle collision on A100",
                    Position = new LatLngLiteral(52.4870, 13.3520),
                    RoadName = "A100",
                    StartTime = "2025-01-15T08:30:00Z",
                    EndTime = "2025-01-15T12:00:00Z"
                },
                new()
                {
                    Type = "construction",
                    Severity = 2,
                    Description = "Road works on Friedrichstraße",
                    Position = new LatLngLiteral(52.5200, 13.3880),
                    RoadName = "Friedrichstraße"
                },
                new()
                {
                    Type = "congestion",
                    Severity = 1,
                    Description = "Slow traffic on Unter den Linden",
                    Position = new LatLngLiteral(52.5170, 13.3890),
                    RoadName = "Unter den Linden"
                }
            }
        });
        var service = new TrafficService(JsRuntime);

        var result = await service.GetTrafficIncidentsAsync(52.55, 52.48, 13.45, 13.35);

        Assert.That(result.Incidents, Has.Count.EqualTo(3));
        Assert.That(result.Incidents![0].Type, Is.EqualTo("accident"));
        Assert.That(result.Incidents[0].Severity, Is.EqualTo(3));
        Assert.That(result.Incidents[0].RoadName, Is.EqualTo("A100"));
        Assert.That(result.Incidents[1].Type, Is.EqualTo("construction"));
        Assert.That(result.Incidents[1].Severity, Is.EqualTo(2));
        Assert.That(result.Incidents[2].Type, Is.EqualTo("congestion"));
        Assert.That(result.Incidents[2].Severity, Is.EqualTo(1));
    }

    [Test]
    public async Task GetTrafficFlowAsync_WithFlowData_ReturnsSpeedsAndJamFactor()
    {
        MockJsResult("blazorHerePlatform.objectManager.getTrafficFlow", new TrafficFlowResult
        {
            Items = new List<TrafficFlowItem>
            {
                new()
                {
                    CurrentSpeed = 55.0,
                    FreeFlowSpeed = 60.0,
                    JamFactor = 0.5,
                    RoadName = "Unter den Linden",
                    Position = new LatLngLiteral(52.5170, 13.3890)
                },
                new()
                {
                    CurrentSpeed = 12.0,
                    FreeFlowSpeed = 50.0,
                    JamFactor = 8.0,
                    RoadName = "A100",
                    Position = new LatLngLiteral(52.4870, 13.3520)
                }
            }
        });
        var service = new TrafficService(JsRuntime);

        var result = await service.GetTrafficFlowAsync(52.55, 52.48, 13.45, 13.35);

        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.Items![0].CurrentSpeed, Is.EqualTo(55.0));
        Assert.That(result.Items[0].FreeFlowSpeed, Is.EqualTo(60.0));
        Assert.That(result.Items[0].JamFactor, Is.EqualTo(0.5));
        Assert.That(result.Items[1].CurrentSpeed, Is.EqualTo(12.0));
        Assert.That(result.Items[1].JamFactor, Is.EqualTo(8.0));
    }
}
