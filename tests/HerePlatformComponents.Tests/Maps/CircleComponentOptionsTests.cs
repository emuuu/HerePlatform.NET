using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class CircleComponentOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new CircleComponent();

        Assert.That(component.CenterLat, Is.EqualTo(0.0));
        Assert.That(component.CenterLng, Is.EqualTo(0.0));
        Assert.That(component.Radius, Is.EqualTo(0.0));
        Assert.That(component.StrokeColor, Is.Null);
        Assert.That(component.FillColor, Is.Null);
        Assert.That(component.LineWidth, Is.Null);
        Assert.That(component.Precision, Is.Null);
        Assert.That(component.Clickable, Is.False);
        Assert.That(component.Visible, Is.True);
        Assert.That(component.Data, Is.Null);
        Assert.That(component.LineCap, Is.Null);
        Assert.That(component.LineJoin, Is.Null);
        Assert.That(component.LineDash, Is.Null);
        Assert.That(component.LineDashOffset, Is.Null);
        Assert.That(component.ZIndex, Is.Null);
        Assert.That(component.Draggable, Is.False);
    }

    [Test]
    public void Guid_IsUnique()
    {
        var a = new CircleComponent();
        var b = new CircleComponent();

        Assert.That(a.Guid, Is.Not.EqualTo(b.Guid));
    }

    [Test]
    public void CenterLat_IsSettable()
    {
        var component = new CircleComponent();
        component.CenterLat = 52.5163;

        Assert.That(component.CenterLat, Is.EqualTo(52.5163));
    }

    [Test]
    public void CenterLng_IsSettable()
    {
        var component = new CircleComponent();
        component.CenterLng = 13.3777;

        Assert.That(component.CenterLng, Is.EqualTo(13.3777));
    }

    [Test]
    public void Radius_IsSettable()
    {
        var component = new CircleComponent();
        component.Radius = 500.0;

        Assert.That(component.Radius, Is.EqualTo(500.0));
    }

    [Test]
    public void StrokeColor_IsSettable()
    {
        var component = new CircleComponent();
        component.StrokeColor = "#FF0000";

        Assert.That(component.StrokeColor, Is.EqualTo("#FF0000"));
    }

    [Test]
    public void FillColor_IsSettable()
    {
        var component = new CircleComponent();
        component.FillColor = "rgba(255, 0, 0, 0.2)";

        Assert.That(component.FillColor, Is.EqualTo("rgba(255, 0, 0, 0.2)"));
    }

    [Test]
    public void Precision_IsSettable()
    {
        var component = new CircleComponent();
        component.Precision = 120;

        Assert.That(component.Precision, Is.EqualTo(120));
    }

    [Test]
    public void Visible_DefaultIsTrue()
    {
        var component = new CircleComponent();

        Assert.That(component.Visible, Is.True);
    }

    [Test]
    public void Data_IsSettable()
    {
        var component = new CircleComponent();
        var data = new { Name = "Zone A" };
        component.Data = data;

        Assert.That(component.Data, Is.SameAs(data));
    }

    [Test]
    public void LineCap_IsSettable()
    {
        var component = new CircleComponent();
        component.LineCap = "round";

        Assert.That(component.LineCap, Is.EqualTo("round"));
    }

    [Test]
    public void LineJoin_IsSettable()
    {
        var component = new CircleComponent();
        component.LineJoin = "bevel";

        Assert.That(component.LineJoin, Is.EqualTo("bevel"));
    }

    [Test]
    public void LineDash_IsSettable()
    {
        var component = new CircleComponent();
        component.LineDash = [6, 3];

        Assert.That(component.LineDash, Is.EqualTo(new double[] { 6, 3 }));
    }

    [Test]
    public void LineDashOffset_IsSettable()
    {
        var component = new CircleComponent();
        component.LineDashOffset = 2.5;

        Assert.That(component.LineDashOffset, Is.EqualTo(2.5));
    }

    [Test]
    public void ZIndex_IsSettable()
    {
        var component = new CircleComponent();
        component.ZIndex = 3;

        Assert.That(component.ZIndex, Is.EqualTo(3));
    }

    [Test]
    public void Draggable_IsSettable()
    {
        var component = new CircleComponent();
        component.Draggable = true;

        Assert.That(component.Draggable, Is.True);
    }

    [Test]
    public async Task HandleGeometryChanged_UpdatesCenterLatLng()
    {
        var component = new CircleComponent();
        component.CenterLat = 0;
        component.CenterLng = 0;

        await component.HandleGeometryChanged(52.5163, 13.3777);

        Assert.That(component.CenterLat, Is.EqualTo(52.5163));
        Assert.That(component.CenterLng, Is.EqualTo(13.3777));
    }

    [Test]
    public void Serialization_OptionsValues()
    {
        var json = Helper.SerializeObject(new
        {
            centerLat = 52.5163,
            centerLng = 13.3777,
            radius = 500.0,
            strokeColor = "#FF0000",
            fillColor = "rgba(255, 0, 0, 0.2)",
            lineWidth = 2.0,
            precision = 120
        });

        Assert.That(json, Does.Contain("\"centerLat\":52.5163"));
        Assert.That(json, Does.Contain("\"radius\":500"));
        Assert.That(json, Does.Contain("\"strokeColor\":\"#FF0000\""));
        Assert.That(json, Does.Contain("\"precision\":120"));
    }
}
