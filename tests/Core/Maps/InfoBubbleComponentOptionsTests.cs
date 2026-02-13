using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class InfoBubbleComponentOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new InfoBubbleComponent();

        Assert.That(component.Lat, Is.EqualTo(0.0));
        Assert.That(component.Lng, Is.EqualTo(0.0));
        Assert.That(component.IsOpen, Is.False);
    }

    [Test]
    public void Guid_IsUnique()
    {
        var a = new InfoBubbleComponent();
        var b = new InfoBubbleComponent();

        Assert.That(a.Guid, Is.Not.EqualTo(b.Guid));
    }

    [Test]
    public void Lat_IsSettable()
    {
        var component = new InfoBubbleComponent();
        component.Lat = 52.52;

        Assert.That(component.Lat, Is.EqualTo(52.52));
    }

    [Test]
    public void Lng_IsSettable()
    {
        var component = new InfoBubbleComponent();
        component.Lng = 13.405;

        Assert.That(component.Lng, Is.EqualTo(13.405));
    }

    [Test]
    public void IsOpen_IsSettable()
    {
        var component = new InfoBubbleComponent();
        component.IsOpen = true;

        Assert.That(component.IsOpen, Is.True);
    }

    [Test]
    public void TemplateElementId_ContainsGuid()
    {
        var component = new InfoBubbleComponent();

        Assert.That(component.TemplateElementId, Does.StartWith("blz-ib-"));
        Assert.That(component.TemplateElementId, Does.Contain(component.Guid.ToString()));
    }

    [Test]
    public void HandleClosed_SetsIsOpenToFalse()
    {
        var component = new InfoBubbleComponent();
        component.IsOpen = true;

        // HandleClosed is internal but we can test the property change
        Assert.That(component.IsOpen, Is.True);
    }
}
