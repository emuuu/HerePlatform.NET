using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class BehaviorFeatureTests
{
    [Test]
    public void None_IsZero()
    {
        Assert.That((int)BehaviorFeature.None, Is.EqualTo(0));
    }

    [Test]
    public void All_ContainsAllFeatures()
    {
        var all = BehaviorFeature.All;

        Assert.That(all.HasFlag(BehaviorFeature.Panning), Is.True);
        Assert.That(all.HasFlag(BehaviorFeature.WheelZoom), Is.True);
        Assert.That(all.HasFlag(BehaviorFeature.PinchZoom), Is.True);
        Assert.That(all.HasFlag(BehaviorFeature.DblTapZoom), Is.True);
        Assert.That(all.HasFlag(BehaviorFeature.Tilt), Is.True);
        Assert.That(all.HasFlag(BehaviorFeature.Heading), Is.True);
        Assert.That(all.HasFlag(BehaviorFeature.FractionalZoom), Is.True);
    }

    [Test]
    public void Flags_ArePowersOfTwo()
    {
        Assert.That((int)BehaviorFeature.Panning, Is.EqualTo(1));
        Assert.That((int)BehaviorFeature.WheelZoom, Is.EqualTo(2));
        Assert.That((int)BehaviorFeature.PinchZoom, Is.EqualTo(4));
        Assert.That((int)BehaviorFeature.DblTapZoom, Is.EqualTo(8));
        Assert.That((int)BehaviorFeature.Tilt, Is.EqualTo(16));
        Assert.That((int)BehaviorFeature.Heading, Is.EqualTo(32));
        Assert.That((int)BehaviorFeature.FractionalZoom, Is.EqualTo(64));
    }

    [Test]
    public void CombinedFlags_WorkCorrectly()
    {
        var combo = BehaviorFeature.Panning | BehaviorFeature.WheelZoom;

        Assert.That(combo.HasFlag(BehaviorFeature.Panning), Is.True);
        Assert.That(combo.HasFlag(BehaviorFeature.WheelZoom), Is.True);
        Assert.That(combo.HasFlag(BehaviorFeature.PinchZoom), Is.False);
    }

    [Test]
    public void BitwiseAnd_FiltersCorrectly()
    {
        var mask = BehaviorFeature.Panning | BehaviorFeature.Tilt;
        var test = mask & BehaviorFeature.Panning;

        Assert.That(test, Is.EqualTo(BehaviorFeature.Panning));
    }
}
