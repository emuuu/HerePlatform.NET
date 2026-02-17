using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geofencing;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class GeofencingServiceTests
{
    private readonly RestGeofencingService _service = new();

    // Polygon around central Berlin (roughly)
    private static GeofenceZone BerlinPolygon() => new()
    {
        Id = "berlin",
        Type = "polygon",
        Vertices =
        [
            new LatLngLiteral(52.54, 13.35),
            new LatLngLiteral(52.54, 13.45),
            new LatLngLiteral(52.50, 13.45),
            new LatLngLiteral(52.50, 13.35)
        ]
    };

    // Circle around Munich center (1km radius)
    private static GeofenceZone MunichCircle() => new()
    {
        Id = "munich",
        Type = "circle",
        Center = new LatLngLiteral(48.1351, 11.5820),
        Radius = 1000
    };

    [Test]
    public async Task CheckPositionAsync_InsidePolygon_ReturnsTrue()
    {
        var position = new LatLngLiteral(52.52, 13.40); // inside Berlin polygon
        var result = await _service.CheckPositionAsync(position, [BerlinPolygon()]);

        Assert.That(result.IsInside, Is.True);
        Assert.That(result.MatchedZoneIds, Contains.Item("berlin"));
    }

    [Test]
    public async Task CheckPositionAsync_OutsidePolygon_ReturnsFalse()
    {
        var position = new LatLngLiteral(48.0, 11.0); // Munich area, outside Berlin polygon
        var result = await _service.CheckPositionAsync(position, [BerlinPolygon()]);

        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Is.Empty);
    }

    [Test]
    public async Task CheckPositionAsync_InsideCircle_ReturnsTrue()
    {
        var position = new LatLngLiteral(48.1355, 11.5825); // very close to Munich center
        var result = await _service.CheckPositionAsync(position, [MunichCircle()]);

        Assert.That(result.IsInside, Is.True);
        Assert.That(result.MatchedZoneIds, Contains.Item("munich"));
    }

    [Test]
    public async Task CheckPositionAsync_OutsideCircle_ReturnsFalse()
    {
        var position = new LatLngLiteral(48.2, 11.6); // ~7km from Munich center
        var result = await _service.CheckPositionAsync(position, [MunichCircle()]);

        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Is.Empty);
    }

    [Test]
    public async Task CheckPositionAsync_MultipleZones_ReturnsAllMatches()
    {
        var position = new LatLngLiteral(52.52, 13.40); // inside Berlin polygon

        var zones = new List<GeofenceZone> { BerlinPolygon(), MunichCircle() };
        var result = await _service.CheckPositionAsync(position, zones);

        Assert.That(result.IsInside, Is.True);
        Assert.That(result.MatchedZoneIds, Has.Count.EqualTo(1));
        Assert.That(result.MatchedZoneIds, Contains.Item("berlin"));
    }

    [Test]
    public async Task CheckPositionAsync_EmptyZones_NoMatches()
    {
        var position = new LatLngLiteral(52.52, 13.40);
        var result = await _service.CheckPositionAsync(position, []);

        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Is.Empty);
    }

    [Test]
    public async Task CheckPositionAsync_PolygonWithNullVertices_NoMatch()
    {
        var zone = new GeofenceZone { Id = "invalid", Type = "polygon", Vertices = null };
        var position = new LatLngLiteral(52.52, 13.40);

        var result = await _service.CheckPositionAsync(position, [zone]);

        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Is.Empty);
    }

    [Test]
    public async Task CheckPositionAsync_CircleWithNoCenter_NoMatch()
    {
        var zone = new GeofenceZone { Id = "no-center", Type = "circle", Radius = 1000 };
        var position = new LatLngLiteral(52.52, 13.40);

        var result = await _service.CheckPositionAsync(position, [zone]);

        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Is.Empty);
    }

    [Test]
    public async Task CheckPositionAsync_ZoneWithNullId_NotAddedToMatches()
    {
        var zone = new GeofenceZone
        {
            Id = null,
            Type = "polygon",
            Vertices =
            [
                new LatLngLiteral(52.54, 13.35),
                new LatLngLiteral(52.54, 13.45),
                new LatLngLiteral(52.50, 13.45),
                new LatLngLiteral(52.50, 13.35)
            ]
        };
        var position = new LatLngLiteral(52.52, 13.40); // inside the polygon

        var result = await _service.CheckPositionAsync(position, [zone]);

        // Position is geometrically inside, but zone has no Id so nothing is added to MatchedZoneIds
        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Is.Empty);
    }
}
