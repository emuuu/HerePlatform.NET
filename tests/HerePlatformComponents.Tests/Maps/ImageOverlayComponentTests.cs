using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class ImageOverlayComponentTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new ImageOverlayComponent();

        Assert.That(component.ImageUrl, Is.Null);
        Assert.That(component.Top, Is.EqualTo(0));
        Assert.That(component.Left, Is.EqualTo(0));
        Assert.That(component.Bottom, Is.EqualTo(0));
        Assert.That(component.Right, Is.EqualTo(0));
        Assert.That(component.Opacity, Is.EqualTo(1.0));
        Assert.That(component.Visible, Is.True);
        Assert.That(component.Guid, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void UniqueGuids()
    {
        var c1 = new ImageOverlayComponent();
        var c2 = new ImageOverlayComponent();

        Assert.That(c1.Guid, Is.Not.EqualTo(c2.Guid));
    }
}
