using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Coordinates;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class MapOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new MapOptions();

        Assert.That(opts.Center, Is.Null);
        Assert.That(opts.Zoom, Is.EqualTo(10));
        Assert.That(opts.LayerType, Is.EqualTo(MapLayerType.VectorNormalMap));
        Assert.That(opts.EnableInteraction, Is.True);
        Assert.That(opts.EnableUI, Is.True);
        Assert.That(opts.MinZoom, Is.Null);
        Assert.That(opts.MaxZoom, Is.Null);
        Assert.That(opts.Tilt, Is.Null);
        Assert.That(opts.Heading, Is.Null);
        Assert.That(opts.Padding, Is.Null);
        Assert.That(opts.AutoColor, Is.Null);
        Assert.That(opts.PixelRatio, Is.Null);
        Assert.That(opts.FixedCenter, Is.Null);
        Assert.That(opts.ApiLoadOptions, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var center = new LatLngLiteral(52.52, 13.405);
        var apiOpts = new HereApiLoadOptions("test-key");
        var padding = new Padding(10, 20, 30, 40);

        var opts = new MapOptions
        {
            Center = center,
            Zoom = 14,
            LayerType = MapLayerType.RasterSatelliteMap,
            EnableInteraction = false,
            EnableUI = false,
            MinZoom = 2,
            MaxZoom = 20,
            Tilt = 45,
            Heading = 90,
            Padding = padding,
            AutoColor = false,
            PixelRatio = 2.0,
            FixedCenter = true,
            ApiLoadOptions = apiOpts
        };

        Assert.That(opts.Center, Is.EqualTo(center));
        Assert.That(opts.Zoom, Is.EqualTo(14));
        Assert.That(opts.LayerType, Is.EqualTo(MapLayerType.RasterSatelliteMap));
        Assert.That(opts.EnableInteraction, Is.False);
        Assert.That(opts.EnableUI, Is.False);
        Assert.That(opts.MinZoom, Is.EqualTo(2));
        Assert.That(opts.MaxZoom, Is.EqualTo(20));
        Assert.That(opts.Tilt, Is.EqualTo(45));
        Assert.That(opts.Heading, Is.EqualTo(90));
        Assert.That(opts.Padding, Is.SameAs(padding));
        Assert.That(opts.AutoColor, Is.False);
        Assert.That(opts.PixelRatio, Is.EqualTo(2.0));
        Assert.That(opts.FixedCenter, Is.True);
        Assert.That(opts.ApiLoadOptions, Is.SameAs(apiOpts));
    }

    [Test]
    public void Validate_ValidDefaults_DoesNotThrow()
    {
        var opts = new MapOptions();
        Assert.DoesNotThrow(() => opts.Validate());
    }

    [Test]
    public void Validate_ZoomWithinBounds_DoesNotThrow()
    {
        var opts = new MapOptions { Zoom = 10, MinZoom = 2, MaxZoom = 20 };
        Assert.DoesNotThrow(() => opts.Validate());
    }

    [Test]
    public void Validate_ZoomBelowMinZoom_Throws()
    {
        var opts = new MapOptions { Zoom = 1, MinZoom = 5 };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => opts.Validate());
        Assert.That(ex!.ParamName, Is.EqualTo("Zoom"));
    }

    [Test]
    public void Validate_ZoomAboveMaxZoom_Throws()
    {
        var opts = new MapOptions { Zoom = 25, MaxZoom = 20 };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => opts.Validate());
        Assert.That(ex!.ParamName, Is.EqualTo("Zoom"));
    }

    [Test]
    public void Validate_MinZoomGreaterThanMaxZoom_Throws()
    {
        var opts = new MapOptions { Zoom = 10, MinZoom = 15, MaxZoom = 5 };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => opts.Validate());
        Assert.That(ex!.ParamName, Is.EqualTo("MinZoom"));
    }

    [Test]
    public void Validate_ZoomEqualsMinZoom_DoesNotThrow()
    {
        var opts = new MapOptions { Zoom = 5, MinZoom = 5 };
        Assert.DoesNotThrow(() => opts.Validate());
    }

    [Test]
    public void Validate_ZoomEqualsMaxZoom_DoesNotThrow()
    {
        var opts = new MapOptions { Zoom = 20, MaxZoom = 20 };
        Assert.DoesNotThrow(() => opts.Validate());
    }

    [Test]
    public void Validate_OnlyMinZoomSet_DoesNotThrow()
    {
        var opts = new MapOptions { Zoom = 10, MinZoom = 2 };
        Assert.DoesNotThrow(() => opts.Validate());
    }

    [Test]
    public void Validate_OnlyMaxZoomSet_DoesNotThrow()
    {
        var opts = new MapOptions { Zoom = 10, MaxZoom = 20 };
        Assert.DoesNotThrow(() => opts.Validate());
    }
}
