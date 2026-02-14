using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services;
using HerePlatformComponents.Maps.Services.Transit;

namespace HerePlatformComponents.Tests.Services.Transit;

[TestFixture]
public class PublicTransitServiceIntegrationTests : ServiceTestBase
{
    [Test]
    public async Task GetDeparturesAsync_WithDepartures_ReturnsTransitInfo()
    {
        MockJsResult("blazorHerePlatform.objectManager.getTransitDepartures", new TransitDeparturesResult
        {
            Departures = new List<TransitDeparture>
            {
                new()
                {
                    LineName = "M5",
                    Direction = "Hohenschönhausen, Zingster Str.",
                    DepartureTime = "2025-01-15T14:32:00+01:00",
                    TransportType = "bus",
                    StationName = "Berlin Hauptbahnhof"
                },
                new()
                {
                    LineName = "U5",
                    Direction = "Hönow",
                    DepartureTime = "2025-01-15T14:35:00+01:00",
                    TransportType = "subway",
                    StationName = "Berlin Hauptbahnhof"
                },
                new()
                {
                    LineName = "M8",
                    Direction = "Ahrensfelde/Stadtgrenze",
                    DepartureTime = "2025-01-15T14:37:00+01:00",
                    TransportType = "tram",
                    StationName = "Berlin Hauptbahnhof"
                }
            }
        });
        var service = new PublicTransitService(JsRuntime);

        var result = await service.GetDeparturesAsync(new LatLngLiteral(52.5251, 13.3694));

        Assert.That(result.Departures, Has.Count.EqualTo(3));
        Assert.That(result.Departures![0].LineName, Is.EqualTo("M5"));
        Assert.That(result.Departures[0].Direction, Does.Contain("Hohenschönhausen"));
        Assert.That(result.Departures[0].TransportType, Is.EqualTo("bus"));
        Assert.That(result.Departures[1].LineName, Is.EqualTo("U5"));
        Assert.That(result.Departures[1].TransportType, Is.EqualTo("subway"));
        Assert.That(result.Departures[2].TransportType, Is.EqualTo("tram"));
    }

    [Test]
    public async Task SearchStationsAsync_WithStations_ReturnsStationData()
    {
        MockJsResult("blazorHerePlatform.objectManager.searchTransitStations", new TransitStationsResult
        {
            Stations = new List<TransitStation>
            {
                new()
                {
                    Name = "Berlin Hauptbahnhof",
                    Position = new LatLngLiteral(52.5251, 13.3694),
                    Distance = 120,
                    TransportTypes = new List<string> { "bus", "subway", "tram", "rail" }
                },
                new()
                {
                    Name = "Invalidenpark",
                    Position = new LatLngLiteral(52.5290, 13.3770),
                    Distance = 380,
                    TransportTypes = new List<string> { "bus", "tram" }
                }
            }
        });
        var service = new PublicTransitService(JsRuntime);

        var result = await service.SearchStationsAsync(new LatLngLiteral(52.5260, 13.3700));

        Assert.That(result.Stations, Has.Count.EqualTo(2));
        Assert.That(result.Stations![0].Name, Is.EqualTo("Berlin Hauptbahnhof"));
        Assert.That(result.Stations[0].Distance, Is.EqualTo(120));
        Assert.That(result.Stations[0].TransportTypes, Has.Count.EqualTo(4));
        Assert.That(result.Stations[0].TransportTypes, Contains.Item("subway"));
        Assert.That(result.Stations[1].Name, Is.EqualTo("Invalidenpark"));
        Assert.That(result.Stations[1].TransportTypes, Has.Count.EqualTo(2));
    }
}
