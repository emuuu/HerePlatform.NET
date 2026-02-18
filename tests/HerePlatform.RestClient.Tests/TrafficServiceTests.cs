using System.Net;
using HerePlatform.Core.Exceptions;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class TrafficServiceTests
{
    private static RestTrafficService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestTrafficService(factory);
    }

    // --- Incidents ---

    [Test]
    public async Task GetTrafficIncidentsAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"results":[]}""");
        var service = CreateService(handler);

        await service.GetTrafficIncidentsAsync(52.6, 52.4, 13.5, 13.3);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://data.traffic.hereapi.com/v7/incidents?"));
        Assert.That(url, Does.Contain("in=bbox%3A13.3%2C52.4%2C13.5%2C52.6"));
    }

    [Test]
    public async Task GetTrafficIncidentsAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "results": [
                {
                    "location": {
                        "description": "A100 Berlin",
                        "shape": {"lat": 52.5, "lng": 13.4}
                    },
                    "incidentDetails": {
                        "type": "accident",
                        "criticality": "major",
                        "description": "Multi-vehicle accident",
                        "startTime": "2024-01-15T08:00:00Z",
                        "endTime": "2024-01-15T10:00:00Z"
                    }
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetTrafficIncidentsAsync(53, 52, 14, 13);

        Assert.That(result.Incidents, Has.Count.EqualTo(1));
        var incident = result.Incidents![0];
        Assert.That(incident.Type, Is.EqualTo("accident"));
        Assert.That(incident.Severity, Is.EqualTo(3)); // "major" → 3
        Assert.That(incident.Description, Is.EqualTo("Multi-vehicle accident"));
        Assert.That(incident.RoadName, Is.EqualTo("A100 Berlin"));
        Assert.That(incident.Position!.Value.Lat, Is.EqualTo(52.5));
        Assert.That(incident.StartTime, Is.EqualTo("2024-01-15T08:00:00Z"));
        Assert.That(incident.EndTime, Is.EqualTo("2024-01-15T10:00:00Z"));
    }

    [Test]
    public async Task GetTrafficIncidentsAsync_CriticalityMapping()
    {
        var json = """
        {
            "results": [
                {"incidentDetails": {"criticality": "critical"}, "location": {}},
                {"incidentDetails": {"criticality": "major"}, "location": {}},
                {"incidentDetails": {"criticality": "minor"}, "location": {}},
                {"incidentDetails": {"criticality": "lowImpact"}, "location": {}},
                {"incidentDetails": {"criticality": "unknown"}, "location": {}}
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetTrafficIncidentsAsync(53, 52, 14, 13);

        Assert.That(result.Incidents![0].Severity, Is.EqualTo(4));
        Assert.That(result.Incidents[1].Severity, Is.EqualTo(3));
        Assert.That(result.Incidents[2].Severity, Is.EqualTo(2));
        Assert.That(result.Incidents[3].Severity, Is.EqualTo(1));
        Assert.That(result.Incidents[4].Severity, Is.EqualTo(0));
    }

    [Test]
    public void GetTrafficIncidentsAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.GetTrafficIncidentsAsync(53, 52, 14, 13));
        Assert.That(ex!.Service, Is.EqualTo("traffic"));
    }

    [Test]
    public async Task GetTrafficIncidentsAsync_EmptyResponse_ReturnsEmpty()
    {
        var handler = MockHttpHandler.WithJson("""{"results":[]}""");
        var service = CreateService(handler);

        var result = await service.GetTrafficIncidentsAsync(53, 52, 14, 13);

        Assert.That(result.Incidents, Is.Not.Null);
        Assert.That(result.Incidents, Is.Empty);
    }

    [Test]
    public void GetTrafficIncidentsAsync_400_ThrowsHereApiException()
    {
        var handler = MockHttpHandler.WithJson("""{"error":"Bad request"}""", HttpStatusCode.BadRequest);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiException>(
            () => service.GetTrafficIncidentsAsync(53, 52, 14, 13));
        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Service, Is.EqualTo("traffic"));
        Assert.That(ex.ErrorBody, Does.Contain("Bad request"));
    }

    // --- Flow ---

    [Test]
    public async Task GetTrafficFlowAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"results":[]}""");
        var service = CreateService(handler);

        await service.GetTrafficFlowAsync(52.6, 52.4, 13.5, 13.3);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://data.traffic.hereapi.com/v7/flow?"));
        Assert.That(url, Does.Contain("in=bbox%3A13.3%2C52.4%2C13.5%2C52.6"));
    }

    [Test]
    public async Task GetTrafficFlowAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "results": [
                {
                    "location": {
                        "description": "Friedrichstr.",
                        "shape": {"lat": 52.52, "lng": 13.39}
                    },
                    "currentFlow": {
                        "speed": 35.5,
                        "freeFlow": 50.0,
                        "jamFactor": 3.2
                    }
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GetTrafficFlowAsync(53, 52, 14, 13);

        Assert.That(result.Items, Has.Count.EqualTo(1));
        var item = result.Items![0];
        Assert.That(item.CurrentSpeed, Is.EqualTo(35.5));
        Assert.That(item.FreeFlowSpeed, Is.EqualTo(50.0));
        Assert.That(item.JamFactor, Is.EqualTo(3.2));
        Assert.That(item.RoadName, Is.EqualTo("Friedrichstr."));
        Assert.That(item.Position!.Value.Lat, Is.EqualTo(52.52));
    }

    [Test]
    public void GetTrafficFlowAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.GetTrafficFlowAsync(53, 52, 14, 13));
        Assert.That(ex!.Service, Is.EqualTo("traffic"));
    }

    [Test]
    public void GetTrafficFlowAsync_400_ThrowsHereApiException()
    {
        var handler = MockHttpHandler.WithJson("""{"error":"Invalid bbox"}""", HttpStatusCode.BadRequest);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiException>(
            () => service.GetTrafficFlowAsync(53, 52, 14, 13));
        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Service, Is.EqualTo("traffic"));
    }
}
