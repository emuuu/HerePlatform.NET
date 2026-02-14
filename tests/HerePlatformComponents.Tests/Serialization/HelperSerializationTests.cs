using HerePlatformComponents;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Serialization;

[TestFixture]
public class HelperSerializationTests
{
    [Test]
    public void SerializeObject_MapOptions_ProducesCamelCaseJson()
    {
        var opts = new MapOptions
        {
            Zoom = 14,
            EnableInteraction = true,
            EnableUI = false
        };

        var json = Helper.SerializeObject(opts);

        Assert.That(json, Does.Contain("\"zoom\":14"));
        Assert.That(json, Does.Contain("\"enableInteraction\":true"));
        Assert.That(json, Does.Contain("\"enableUI\":false"));
    }

    [Test]
    public void SerializeObject_NullProperties_AreOmitted()
    {
        var opts = new MapOptions();

        var json = Helper.SerializeObject(opts);

        Assert.That(json, Does.Not.Contain("\"center\""));
        Assert.That(json, Does.Not.Contain("\"minZoom\""));
        Assert.That(json, Does.Not.Contain("\"maxZoom\""));
        Assert.That(json, Does.Not.Contain("\"tilt\""));
        Assert.That(json, Does.Not.Contain("\"heading\""));
    }

    [Test]
    public void SerializeObject_StyleOptions_ProducesCorrectJson()
    {
        var style = new StyleOptions
        {
            StrokeColor = "#FF0000",
            LineWidth = 2.5
        };

        var json = Helper.SerializeObject(style);

        Assert.That(json, Does.Contain("\"strokeColor\":\"#FF0000\""));
        Assert.That(json, Does.Contain("\"lineWidth\":2.5"));
        Assert.That(json, Does.Not.Contain("\"fillColor\""));
    }

    [Test]
    public void DeSerializeObject_Generic_ParsesCorrectly()
    {
        var json = """{"strokeColor":"blue","lineWidth":3}""";

        var style = Helper.DeSerializeObject<StyleOptions>(json);

        Assert.That(style, Is.Not.Null);
        Assert.That(style!.StrokeColor, Is.EqualTo("blue"));
        Assert.That(style.LineWidth, Is.EqualTo(3));
    }

    [Test]
    public void DeSerializeObject_Null_ReturnsDefault()
    {
        var result = Helper.DeSerializeObject<StyleOptions>(null);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void SerializeObject_CircleOptions_IncludesNestedStyle()
    {
        var opts = new CircleOptions
        {
            Radius = 1000,
            Style = new StyleOptions { FillColor = "green" }
        };

        var json = Helper.SerializeObject(opts);

        Assert.That(json, Does.Contain("\"radius\":1000"));
        Assert.That(json, Does.Contain("\"fillColor\":\"green\""));
    }

    [Test]
    public void RoundTrip_MarkerOptions_PreservesValues()
    {
        var opts = new MarkerOptions
        {
            Draggable = true,
            Visibility = true,
            ZIndex = 10
        };

        var json = Helper.SerializeObject(opts);
        var result = Helper.DeSerializeObject<MarkerOptions>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Draggable, Is.True);
        Assert.That(result.Visibility, Is.True);
        Assert.That(result.ZIndex, Is.EqualTo(10));
    }

    [Test]
    public void SerializeObject_LatLngLiteral_UsesCustomConverter()
    {
        var coord = new LatLngLiteral(52.52, 13.405);

        var json = Helper.SerializeObject(coord);

        Assert.That(json, Does.Contain("\"lat\":52.52"));
        Assert.That(json, Does.Contain("\"lng\":13.405"));
    }
}
