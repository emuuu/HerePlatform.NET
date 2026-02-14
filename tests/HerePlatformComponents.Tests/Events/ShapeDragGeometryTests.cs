using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class ShapeDragGeometryTests
{
    [Test]
    public void DefaultValues_AreNull()
    {
        var geo = new ShapeDragGeometry();

        Assert.That(geo.CenterLat, Is.Null);
        Assert.That(geo.CenterLng, Is.Null);
        Assert.That(geo.Top, Is.Null);
        Assert.That(geo.Left, Is.Null);
        Assert.That(geo.Bottom, Is.Null);
        Assert.That(geo.Right, Is.Null);
        Assert.That(geo.Path, Is.Null);
    }

    [Test]
    public void CircleGeometry_IsSettable()
    {
        var geo = new ShapeDragGeometry
        {
            CenterLat = 52.5163,
            CenterLng = 13.3777
        };

        Assert.That(geo.CenterLat, Is.EqualTo(52.5163));
        Assert.That(geo.CenterLng, Is.EqualTo(13.3777));
    }

    [Test]
    public void RectGeometry_IsSettable()
    {
        var geo = new ShapeDragGeometry
        {
            Top = 52.525,
            Left = 13.410,
            Bottom = 52.515,
            Right = 13.430
        };

        Assert.That(geo.Top, Is.EqualTo(52.525));
        Assert.That(geo.Left, Is.EqualTo(13.410));
        Assert.That(geo.Bottom, Is.EqualTo(52.515));
        Assert.That(geo.Right, Is.EqualTo(13.430));
    }

    [Test]
    public void PathGeometry_IsSettable()
    {
        var path = new List<LatLngLiteral>
        {
            new(52.52, 13.39),
            new(52.53, 13.40),
            new(52.51, 13.41)
        };

        var geo = new ShapeDragGeometry { Path = path };

        Assert.That(geo.Path, Has.Exactly(3).Items);
        Assert.That(geo.Path![0].Lat, Is.EqualTo(52.52));
    }

    [Test]
    public void Deserialization_FromJson_CircleGeometry()
    {
        var json = "{\"centerLat\":52.5163,\"centerLng\":13.3777}";
        var geo = Helper.DeSerializeObject<ShapeDragGeometry>(json);

        Assert.That(geo, Is.Not.Null);
        Assert.That(geo!.CenterLat, Is.EqualTo(52.5163));
        Assert.That(geo.CenterLng, Is.EqualTo(13.3777));
        Assert.That(geo.Top, Is.Null);
        Assert.That(geo.Path, Is.Null);
    }

    [Test]
    public void Deserialization_FromJson_RectGeometry()
    {
        var json = "{\"top\":52.525,\"left\":13.41,\"bottom\":52.515,\"right\":13.43}";
        var geo = Helper.DeSerializeObject<ShapeDragGeometry>(json);

        Assert.That(geo, Is.Not.Null);
        Assert.That(geo!.Top, Is.EqualTo(52.525));
        Assert.That(geo.Left, Is.EqualTo(13.41));
        Assert.That(geo.Bottom, Is.EqualTo(52.515));
        Assert.That(geo.Right, Is.EqualTo(13.43));
    }

    [Test]
    public void Deserialization_FromJson_PathGeometry()
    {
        var json = "{\"path\":[{\"lat\":52.52,\"lng\":13.39},{\"lat\":52.53,\"lng\":13.40}]}";
        var geo = Helper.DeSerializeObject<ShapeDragGeometry>(json);

        Assert.That(geo, Is.Not.Null);
        Assert.That(geo!.Path, Has.Exactly(2).Items);
        Assert.That(geo.Path![0].Lat, Is.EqualTo(52.52));
        Assert.That(geo.Path[1].Lng, Is.EqualTo(13.40));
    }
}
