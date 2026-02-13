using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class PolylineComponentOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new PolylineComponent();

        Assert.That(component.Path, Is.Null);
        Assert.That(component.StrokeColor, Is.Null);
        Assert.That(component.LineWidth, Is.Null);
        Assert.That(component.LineCap, Is.Null);
        Assert.That(component.LineDash, Is.Null);
        Assert.That(component.Clickable, Is.False);
        Assert.That(component.Visible, Is.True);
        Assert.That(component.Data, Is.Null);
    }

    [Test]
    public void Guid_IsUnique()
    {
        var a = new PolylineComponent();
        var b = new PolylineComponent();

        Assert.That(a.Guid, Is.Not.EqualTo(b.Guid));
    }

    [Test]
    public void Path_AcceptsList()
    {
        var component = new PolylineComponent();
        var path = new List<LatLngLiteral>
        {
            new(52.52, 13.39),
            new(52.52, 13.42),
            new(52.51, 13.44)
        };

        component.Path = path;

        Assert.That(component.Path, Has.Exactly(3).Items);
    }

    [Test]
    public void StrokeColor_IsSettable()
    {
        var component = new PolylineComponent();
        component.StrokeColor = "#FF0000";

        Assert.That(component.StrokeColor, Is.EqualTo("#FF0000"));
    }

    [Test]
    public void LineWidth_IsSettable()
    {
        var component = new PolylineComponent();
        component.LineWidth = 5.0;

        Assert.That(component.LineWidth, Is.EqualTo(5.0));
    }

    [Test]
    public void LineCap_IsSettable()
    {
        var component = new PolylineComponent();
        component.LineCap = "round";

        Assert.That(component.LineCap, Is.EqualTo("round"));
    }

    [Test]
    public void LineDash_IsSettable()
    {
        var component = new PolylineComponent();
        component.LineDash = [8, 4];

        Assert.That(component.LineDash, Is.EqualTo(new double[] { 8, 4 }));
    }

    [Test]
    public void Visible_DefaultIsTrue()
    {
        var component = new PolylineComponent();

        Assert.That(component.Visible, Is.True);
    }

    [Test]
    public void Data_IsSettable()
    {
        var component = new PolylineComponent();
        var data = new { Name = "Route A" };
        component.Data = data;

        Assert.That(component.Data, Is.SameAs(data));
    }

    [Test]
    public void Serialization_OptionsStruct_HasCorrectProperties()
    {
        var json = Helper.SerializeObject(new
        {
            path = new List<LatLngLiteral> { new(52.52, 13.39) },
            strokeColor = "#0066FF",
            lineWidth = 4.0,
            lineCap = "round",
            lineDash = new double[] { 8, 4 },
            clickable = true,
            visible = true
        });

        Assert.That(json, Does.Contain("\"strokeColor\":\"#0066FF\""));
        Assert.That(json, Does.Contain("\"lineWidth\":4"));
        Assert.That(json, Does.Contain("\"lineCap\":\"round\""));
        Assert.That(json, Does.Contain("\"lineDash\":[8,4]"));
    }
}
