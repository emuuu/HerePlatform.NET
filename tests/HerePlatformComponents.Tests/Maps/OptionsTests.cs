using HerePlatformComponents.Maps;
using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Coordinates;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class MarkerOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new MarkerOptions();

        Assert.That(opts.Position, Is.Null);
        Assert.That(opts.Icon, Is.Null);
        Assert.That(opts.Data, Is.Null);
        Assert.That(opts.Draggable, Is.Null);
        Assert.That(opts.Visibility, Is.Null);
        Assert.That(opts.ZIndex, Is.Null);
        Assert.That(opts.Min, Is.Null);
        Assert.That(opts.Max, Is.Null);
        Assert.That(opts.Volatility, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var pos = new LatLngLiteral(52.52, 13.405);
        var opts = new MarkerOptions
        {
            Position = pos,
            Icon = "marker.svg",
            Data = new { Name = "Test" },
            Draggable = true,
            Visibility = true,
            ZIndex = 5,
            Min = 3,
            Max = 18,
            Volatility = true
        };

        Assert.That(opts.Position, Is.EqualTo(pos));
        Assert.That(opts.Icon, Is.EqualTo("marker.svg"));
        Assert.That(opts.Data, Is.Not.Null);
        Assert.That(opts.Draggable, Is.True);
        Assert.That(opts.Visibility, Is.True);
        Assert.That(opts.ZIndex, Is.EqualTo(5));
        Assert.That(opts.Min, Is.EqualTo(3));
        Assert.That(opts.Max, Is.EqualTo(18));
        Assert.That(opts.Volatility, Is.True);
    }
}

[TestFixture]
public class CircleOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new CircleOptions();

        Assert.That(opts.Center, Is.Null);
        Assert.That(opts.Radius, Is.EqualTo(0));
        Assert.That(opts.Style, Is.Null);
        Assert.That(opts.Precision, Is.Null);
        Assert.That(opts.Extrusion, Is.Null);
        Assert.That(opts.Elevation, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var center = new LatLngLiteral(52.52, 13.405);
        var style = new StyleOptions { StrokeColor = "red", FillColor = "blue" };

        var opts = new CircleOptions
        {
            Center = center,
            Radius = 500.0,
            Style = style,
            Visibility = true,
            Precision = 120,
            Extrusion = 50.0,
            Elevation = 10.0
        };

        Assert.That(opts.Center, Is.EqualTo(center));
        Assert.That(opts.Radius, Is.EqualTo(500.0));
        Assert.That(opts.Style, Is.SameAs(style));
        Assert.That(opts.Visibility, Is.True);
        Assert.That(opts.Precision, Is.EqualTo(120));
        Assert.That(opts.Extrusion, Is.EqualTo(50.0));
        Assert.That(opts.Elevation, Is.EqualTo(10.0));
    }
}

[TestFixture]
public class PolylineOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new PolylineOptions();

        Assert.That(opts.Path, Is.Null);
        Assert.That(opts.Style, Is.Null);
    }

    [Test]
    public void Path_AcceptsList()
    {
        var path = new List<LatLngLiteral>
        {
            new(52.52, 13.405),
            new(48.8566, 2.3522),
            new(51.5074, -0.1278)
        };

        var opts = new PolylineOptions { Path = path };

        Assert.That(opts.Path, Has.Exactly(3).Items);
    }
}

[TestFixture]
public class PolygonOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new PolygonOptions();

        Assert.That(opts.Path, Is.Null);
        Assert.That(opts.Style, Is.Null);
        Assert.That(opts.Extrusion, Is.Null);
        Assert.That(opts.Elevation, Is.Null);
    }

    [Test]
    public void Path_AcceptsTriangle()
    {
        var path = new List<LatLngLiteral>
        {
            new(52.52, 13.405),
            new(52.50, 13.40),
            new(52.51, 13.42)
        };
        var style = new StyleOptions { FillColor = "rgba(0,255,0,0.3)", StrokeColor = "green" };

        var opts = new PolygonOptions { Path = path, Style = style, Extrusion = 100.0, Elevation = 5.0 };

        Assert.That(opts.Path, Has.Exactly(3).Items);
        Assert.That(opts.Style!.FillColor, Is.EqualTo("rgba(0,255,0,0.3)"));
        Assert.That(opts.Extrusion, Is.EqualTo(100.0));
        Assert.That(opts.Elevation, Is.EqualTo(5.0));
    }
}

[TestFixture]
public class RectOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new RectOptions();

        Assert.That(opts.Bounds, Is.Null);
        Assert.That(opts.Style, Is.Null);
        Assert.That(opts.Extrusion, Is.Null);
        Assert.That(opts.Elevation, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var bounds = new GeoRect(52.6, 13.1, 52.4, 13.8);
        var style = new StyleOptions { StrokeColor = "#000", LineWidth = 2 };

        var opts = new RectOptions
        {
            Bounds = bounds,
            Style = style,
            Extrusion = 30.0,
            Elevation = 15.0
        };

        Assert.That(opts.Bounds, Is.SameAs(bounds));
        Assert.That(opts.Style, Is.SameAs(style));
        Assert.That(opts.Extrusion, Is.EqualTo(30.0));
        Assert.That(opts.Elevation, Is.EqualTo(15.0));
    }
}

[TestFixture]
public class ListableEntityOptionsBaseTests
{
    private class TestOptions : ListableEntityOptionsBase { }

    [Test]
    public void DefaultValues_AreAllNull()
    {
        var opts = new TestOptions();

        Assert.That(opts.Visibility, Is.Null);
        Assert.That(opts.ZIndex, Is.Null);
        Assert.That(opts.Min, Is.Null);
        Assert.That(opts.Max, Is.Null);
        Assert.That(opts.Volatility, Is.Null);
        Assert.That(opts.Data, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var data = new { Key = "value" };
        var opts = new TestOptions
        {
            Visibility = true,
            ZIndex = 10,
            Min = 2.0,
            Max = 20.0,
            Volatility = true,
            Data = data
        };

        Assert.That(opts.Visibility, Is.True);
        Assert.That(opts.ZIndex, Is.EqualTo(10));
        Assert.That(opts.Min, Is.EqualTo(2.0));
        Assert.That(opts.Max, Is.EqualTo(20.0));
        Assert.That(opts.Volatility, Is.True);
        Assert.That(opts.Data, Is.SameAs(data));
    }
}

[TestFixture]
public class MapOptionsTests_Extended
{
    [Test]
    public void Padding_DefaultIsNull()
    {
        var opts = new MapOptions();

        Assert.That(opts.Padding, Is.Null);
        Assert.That(opts.AutoColor, Is.Null);
    }

    [Test]
    public void Padding_IsSettable()
    {
        var padding = new Padding(10, 20, 30, 40);
        var opts = new MapOptions
        {
            Padding = padding,
            AutoColor = false
        };

        Assert.That(opts.Padding, Is.SameAs(padding));
        Assert.That(opts.AutoColor, Is.False);
    }
}
