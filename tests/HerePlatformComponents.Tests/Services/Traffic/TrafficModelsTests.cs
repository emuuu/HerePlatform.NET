using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.Traffic;

namespace HerePlatformComponents.Tests.Services.Traffic;

[TestFixture]
public class TrafficModelsTests
{
    [Test]
    public void TrafficIncident_DefaultValues()
    {
        var incident = new TrafficIncident();

        Assert.That(incident.Type, Is.Null);
        Assert.That(incident.Severity, Is.EqualTo(0));
        Assert.That(incident.Description, Is.Null);
        Assert.That(incident.Position, Is.Null);
        Assert.That(incident.RoadName, Is.Null);
        Assert.That(incident.StartTime, Is.Null);
        Assert.That(incident.EndTime, Is.Null);
    }

    [Test]
    public void TrafficIncident_WithValues()
    {
        var incident = new TrafficIncident
        {
            Type = "accident",
            Severity = 3,
            Description = "Multi-vehicle accident",
            Position = new LatLngLiteral(52.52, 13.405),
            RoadName = "A100",
            StartTime = "2026-01-15T08:00:00Z",
            EndTime = "2026-01-15T10:00:00Z"
        };

        Assert.That(incident.Type, Is.EqualTo("accident"));
        Assert.That(incident.Severity, Is.EqualTo(3));
        Assert.That(incident.Description, Is.EqualTo("Multi-vehicle accident"));
        Assert.That(incident.Position!.Value.Lat, Is.EqualTo(52.52));
        Assert.That(incident.RoadName, Is.EqualTo("A100"));
    }

    [Test]
    public void TrafficIncidentsResult_DefaultValues()
    {
        var result = new TrafficIncidentsResult();

        Assert.That(result.Incidents, Is.Null);
    }

    [Test]
    public void TrafficFlowItem_DefaultValues()
    {
        var item = new TrafficFlowItem();

        Assert.That(item.CurrentSpeed, Is.EqualTo(0));
        Assert.That(item.FreeFlowSpeed, Is.EqualTo(0));
        Assert.That(item.JamFactor, Is.EqualTo(0));
        Assert.That(item.RoadName, Is.Null);
        Assert.That(item.Position, Is.Null);
    }

    [Test]
    public void TrafficFlowItem_WithValues()
    {
        var item = new TrafficFlowItem
        {
            CurrentSpeed = 45.5,
            FreeFlowSpeed = 80.0,
            JamFactor = 5.2,
            RoadName = "B1",
            Position = new LatLngLiteral(52.52, 13.405)
        };

        Assert.That(item.CurrentSpeed, Is.EqualTo(45.5));
        Assert.That(item.FreeFlowSpeed, Is.EqualTo(80.0));
        Assert.That(item.JamFactor, Is.EqualTo(5.2));
        Assert.That(item.RoadName, Is.EqualTo("B1"));
    }

    [Test]
    public void TrafficFlowResult_DefaultValues()
    {
        var result = new TrafficFlowResult();

        Assert.That(result.Items, Is.Null);
    }
}
