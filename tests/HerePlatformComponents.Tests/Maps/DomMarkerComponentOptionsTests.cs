using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class DomMarkerComponentOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new DomMarkerComponent();

        Assert.That(component.Lat, Is.EqualTo(0.0));
        Assert.That(component.Lng, Is.EqualTo(0.0));
        Assert.That(component.Clickable, Is.False);
        Assert.That(component.Draggable, Is.False);
        Assert.That(component.Visible, Is.True);
        Assert.That(component.ZIndex, Is.Null);
        Assert.That(component.Data, Is.Null);
    }

    [Test]
    public void Guid_IsUnique()
    {
        var a = new DomMarkerComponent();
        var b = new DomMarkerComponent();

        Assert.That(a.Guid, Is.Not.EqualTo(b.Guid));
    }

    [Test]
    public void Lat_IsSettable()
    {
        var component = new DomMarkerComponent();
        component.Lat = 52.52;

        Assert.That(component.Lat, Is.EqualTo(52.52));
    }

    [Test]
    public void Lng_IsSettable()
    {
        var component = new DomMarkerComponent();
        component.Lng = 13.405;

        Assert.That(component.Lng, Is.EqualTo(13.405));
    }

    [Test]
    public void TemplateElementId_ContainsGuid()
    {
        var component = new DomMarkerComponent();

        Assert.That(component.TemplateElementId, Does.StartWith("blz-dm-"));
        Assert.That(component.TemplateElementId, Does.Contain(component.Guid.ToString()));
    }

    [Test]
    public void Data_IsSettable()
    {
        var component = new DomMarkerComponent();
        var data = new { Id = 42 };
        component.Data = data;

        Assert.That(component.Data, Is.SameAs(data));
    }

    [Test]
    public void Visible_DefaultIsTrue()
    {
        var component = new DomMarkerComponent();

        Assert.That(component.Visible, Is.True);
    }
}
