using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geofencing;
using HerePlatform.Core.Services;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services;

namespace HerePlatformComponents.Tests.Services.Geofencing;

[TestFixture]
public class GeofencingTests
{
    [Test]
    public void GeofenceZone_DefaultValues()
    {
        var zone = new GeofenceZone();

        Assert.That(zone.Id, Is.Null);
        Assert.That(zone.Name, Is.Null);
        Assert.That(zone.Type, Is.EqualTo("polygon"));
        Assert.That(zone.Vertices, Is.Null);
        Assert.That(zone.Center, Is.Null);
        Assert.That(zone.Radius, Is.EqualTo(0));
    }

    [Test]
    public void GeofenceCheckResult_DefaultValues()
    {
        var result = new GeofenceCheckResult();

        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Is.Null);
    }

    [Test]
    public async Task CheckPosition_InsidePolygon()
    {
        var service = new GeofencingService();
        var position = new LatLngLiteral(52.52, 13.405);
        var zones = new List<GeofenceZone>
        {
            new GeofenceZone
            {
                Id = "zone1",
                Name = "Berlin Center",
                Type = "polygon",
                Vertices = new List<LatLngLiteral>
                {
                    new(52.55, 13.35),
                    new(52.55, 13.45),
                    new(52.49, 13.45),
                    new(52.49, 13.35)
                }
            }
        };

        var result = await service.CheckPositionAsync(position, zones);

        Assert.That(result.IsInside, Is.True);
        Assert.That(result.MatchedZoneIds, Has.Count.EqualTo(1));
        Assert.That(result.MatchedZoneIds![0], Is.EqualTo("zone1"));
    }

    [Test]
    public async Task CheckPosition_OutsidePolygon()
    {
        var service = new GeofencingService();
        var position = new LatLngLiteral(48.8566, 2.3522); // Paris - outside Berlin zone
        var zones = new List<GeofenceZone>
        {
            new GeofenceZone
            {
                Id = "zone1",
                Name = "Berlin Center",
                Type = "polygon",
                Vertices = new List<LatLngLiteral>
                {
                    new(52.55, 13.35),
                    new(52.55, 13.45),
                    new(52.49, 13.45),
                    new(52.49, 13.35)
                }
            }
        };

        var result = await service.CheckPositionAsync(position, zones);

        Assert.That(result.IsInside, Is.False);
        Assert.That(result.MatchedZoneIds, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task CheckPosition_InsideCircle()
    {
        var service = new GeofencingService();
        var position = new LatLngLiteral(52.521, 13.406); // Very close to center
        var zones = new List<GeofenceZone>
        {
            new GeofenceZone
            {
                Id = "circle1",
                Name = "Berlin Center Circle",
                Type = "circle",
                Center = new LatLngLiteral(52.52, 13.405),
                Radius = 1000 // 1km radius
            }
        };

        var result = await service.CheckPositionAsync(position, zones);

        Assert.That(result.IsInside, Is.True);
        Assert.That(result.MatchedZoneIds, Has.Count.EqualTo(1));
        Assert.That(result.MatchedZoneIds![0], Is.EqualTo("circle1"));
    }

    [Test]
    public async Task CheckPosition_OutsideCircle()
    {
        var service = new GeofencingService();
        var position = new LatLngLiteral(52.60, 13.50); // ~10km away
        var zones = new List<GeofenceZone>
        {
            new GeofenceZone
            {
                Id = "circle1",
                Name = "Berlin Center Circle",
                Type = "circle",
                Center = new LatLngLiteral(52.52, 13.405),
                Radius = 1000 // 1km radius
            }
        };

        var result = await service.CheckPositionAsync(position, zones);

        Assert.That(result.IsInside, Is.False);
    }

    [Test]
    public async Task CheckPosition_MultipleZones()
    {
        var service = new GeofencingService();
        var position = new LatLngLiteral(52.52, 13.405);
        var zones = new List<GeofenceZone>
        {
            new GeofenceZone
            {
                Id = "zone1",
                Type = "polygon",
                Vertices = new List<LatLngLiteral>
                {
                    new(52.55, 13.35),
                    new(52.55, 13.45),
                    new(52.49, 13.45),
                    new(52.49, 13.35)
                }
            },
            new GeofenceZone
            {
                Id = "zone2",
                Type = "circle",
                Center = new LatLngLiteral(52.52, 13.405),
                Radius = 500
            },
            new GeofenceZone
            {
                Id = "zone3",
                Type = "circle",
                Center = new LatLngLiteral(48.0, 2.0),
                Radius = 100
            }
        };

        var result = await service.CheckPositionAsync(position, zones);

        Assert.That(result.IsInside, Is.True);
        Assert.That(result.MatchedZoneIds, Has.Count.EqualTo(2));
        Assert.That(result.MatchedZoneIds, Contains.Item("zone1"));
        Assert.That(result.MatchedZoneIds, Contains.Item("zone2"));
    }
}
