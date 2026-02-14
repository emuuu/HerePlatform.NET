using HerePlatformComponents.Maps.Coordinates;

namespace HerePlatformComponents.Tests.Coordinates;

[TestFixture]
public class PaddingTests
{
    [Test]
    public void DefaultConstructor_AllZero()
    {
        var padding = new Padding();

        Assert.That(padding.Top, Is.EqualTo(0));
        Assert.That(padding.Right, Is.EqualTo(0));
        Assert.That(padding.Bottom, Is.EqualTo(0));
        Assert.That(padding.Left, Is.EqualTo(0));
    }

    [Test]
    public void ParameterizedConstructor_SetsAllProperties()
    {
        var padding = new Padding(10, 20, 30, 40);

        Assert.That(padding.Top, Is.EqualTo(10));
        Assert.That(padding.Right, Is.EqualTo(20));
        Assert.That(padding.Bottom, Is.EqualTo(30));
        Assert.That(padding.Left, Is.EqualTo(40));
    }

    [Test]
    public void Properties_AreSettable()
    {
        var padding = new Padding { Top = 5, Right = 10, Bottom = 15, Left = 20 };

        Assert.That(padding.Top, Is.EqualTo(5));
        Assert.That(padding.Right, Is.EqualTo(10));
        Assert.That(padding.Bottom, Is.EqualTo(15));
        Assert.That(padding.Left, Is.EqualTo(20));
    }

    [Test]
    public void UniformPadding_AllEqual()
    {
        var padding = new Padding(16, 16, 16, 16);

        Assert.That(padding.Top, Is.EqualTo(padding.Right));
        Assert.That(padding.Right, Is.EqualTo(padding.Bottom));
        Assert.That(padding.Bottom, Is.EqualTo(padding.Left));
    }
}
