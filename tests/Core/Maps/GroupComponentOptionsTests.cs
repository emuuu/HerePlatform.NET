using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class GroupComponentOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new GroupComponent();

        Assert.That(component.Visible, Is.True);
        Assert.That(component.ZIndex, Is.Null);
        Assert.That(component.Data, Is.Null);
    }

    [Test]
    public void Guid_IsUnique()
    {
        var a = new GroupComponent();
        var b = new GroupComponent();

        Assert.That(a.Guid, Is.Not.EqualTo(b.Guid));
    }

    [Test]
    public void Visible_IsSettable()
    {
        var component = new GroupComponent();
        component.Visible = false;

        Assert.That(component.Visible, Is.False);
    }

    [Test]
    public void ZIndex_IsSettable()
    {
        var component = new GroupComponent();
        component.ZIndex = 10;

        Assert.That(component.ZIndex, Is.EqualTo(10));
    }

    [Test]
    public void Data_IsSettable()
    {
        var component = new GroupComponent();
        var data = new { Name = "Layer 1" };
        component.Data = data;

        Assert.That(component.Data, Is.SameAs(data));
    }
}
