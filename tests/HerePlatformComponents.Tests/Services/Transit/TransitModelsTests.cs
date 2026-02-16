using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.Transit;

namespace HerePlatformComponents.Tests.Services.Transit;

[TestFixture]
public class TransitModelsTests
{
    [Test]
    public void TransitDeparture_DefaultValues()
    {
        var departure = new TransitDeparture();

        Assert.That(departure.LineName, Is.Null);
        Assert.That(departure.Direction, Is.Null);
        Assert.That(departure.DepartureTime, Is.Null);
        Assert.That(departure.TransportType, Is.Null);
        Assert.That(departure.StationName, Is.Null);
    }

    [Test]
    public void TransitDeparture_WithValues()
    {
        var departure = new TransitDeparture
        {
            LineName = "U2",
            Direction = "Ruhleben",
            DepartureTime = "2026-01-15T14:30:00",
            TransportType = "subway",
            StationName = "Alexanderplatz"
        };

        Assert.That(departure.LineName, Is.EqualTo("U2"));
        Assert.That(departure.Direction, Is.EqualTo("Ruhleben"));
        Assert.That(departure.TransportType, Is.EqualTo("subway"));
        Assert.That(departure.StationName, Is.EqualTo("Alexanderplatz"));
    }

    [Test]
    public void TransitDeparturesResult_DefaultValues()
    {
        var result = new TransitDeparturesResult();

        Assert.That(result.Departures, Is.Null);
    }

    [Test]
    public void TransitStation_DefaultValues()
    {
        var station = new TransitStation();

        Assert.That(station.Name, Is.Null);
        Assert.That(station.Position, Is.Null);
        Assert.That(station.Distance, Is.EqualTo(0));
        Assert.That(station.TransportTypes, Is.Null);
    }

    [Test]
    public void TransitStation_WithValues()
    {
        var station = new TransitStation
        {
            Name = "Alexanderplatz",
            Position = new LatLngLiteral(52.5219, 13.4132),
            Distance = 150,
            TransportTypes = new List<string> { "subway", "tram", "bus" }
        };

        Assert.That(station.Name, Is.EqualTo("Alexanderplatz"));
        Assert.That(station.Position!.Value.Lat, Is.EqualTo(52.5219));
        Assert.That(station.Distance, Is.EqualTo(150));
        Assert.That(station.TransportTypes, Has.Count.EqualTo(3));
    }

    [Test]
    public void TransitStationsResult_DefaultValues()
    {
        var result = new TransitStationsResult();

        Assert.That(result.Stations, Is.Null);
    }
}
