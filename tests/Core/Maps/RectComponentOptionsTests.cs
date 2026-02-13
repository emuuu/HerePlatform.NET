using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class RectComponentOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new RectComponent();

        Assert.That(component.Top, Is.EqualTo(0.0));
        Assert.That(component.Left, Is.EqualTo(0.0));
        Assert.That(component.Bottom, Is.EqualTo(0.0));
        Assert.That(component.Right, Is.EqualTo(0.0));
        Assert.That(component.StrokeColor, Is.Null);
        Assert.That(component.FillColor, Is.Null);
        Assert.That(component.LineWidth, Is.Null);
        Assert.That(component.Clickable, Is.False);
        Assert.That(component.Visible, Is.True);
        Assert.That(component.Data, Is.Null);
    }

    [Test]
    public void Guid_IsUnique()
    {
        var a = new RectComponent();
        var b = new RectComponent();

        Assert.That(a.Guid, Is.Not.EqualTo(b.Guid));
    }

    [Test]
    public void BoundaryValues_AreSettable()
    {
        var component = new RectComponent();
        component.Top = 52.525;
        component.Left = 13.410;
        component.Bottom = 52.515;
        component.Right = 13.430;

        Assert.That(component.Top, Is.EqualTo(52.525));
        Assert.That(component.Left, Is.EqualTo(13.410));
        Assert.That(component.Bottom, Is.EqualTo(52.515));
        Assert.That(component.Right, Is.EqualTo(13.430));
    }

    [Test]
    public void StrokeColor_IsSettable()
    {
        var component = new RectComponent();
        component.StrokeColor = "#FF9900";

        Assert.That(component.StrokeColor, Is.EqualTo("#FF9900"));
    }

    [Test]
    public void FillColor_IsSettable()
    {
        var component = new RectComponent();
        component.FillColor = "rgba(255, 153, 0, 0.25)";

        Assert.That(component.FillColor, Is.EqualTo("rgba(255, 153, 0, 0.25)"));
    }

    [Test]
    public void LineWidth_IsSettable()
    {
        var component = new RectComponent();
        component.LineWidth = 3.0;

        Assert.That(component.LineWidth, Is.EqualTo(3.0));
    }

    [Test]
    public void Visible_DefaultIsTrue()
    {
        var component = new RectComponent();

        Assert.That(component.Visible, Is.True);
    }

    [Test]
    public void Data_IsSettable()
    {
        var component = new RectComponent();
        var data = new { Name = "Area B" };
        component.Data = data;

        Assert.That(component.Data, Is.SameAs(data));
    }

    [Test]
    public void Serialization_OptionsValues()
    {
        var json = Helper.SerializeObject(new
        {
            top = 52.525,
            left = 13.410,
            bottom = 52.515,
            right = 13.430,
            strokeColor = "#FF9900",
            fillColor = "rgba(255, 153, 0, 0.25)",
            lineWidth = 2.0
        });

        Assert.That(json, Does.Contain("\"top\":52.525"));
        Assert.That(json, Does.Contain("\"left\":13.41"));
        Assert.That(json, Does.Contain("\"strokeColor\":\"#FF9900\""));
    }
}
