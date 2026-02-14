using HerePlatformComponents.Maps.Data;

namespace HerePlatformComponents.Tests.Data;

[TestFixture]
public class GeoJsonLoadedEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new GeoJsonLoadedEventArgs();

        Assert.That(args.ObjectCount, Is.EqualTo(0));
    }

    [Test]
    public void ObjectCount_IsSettable()
    {
        var args = new GeoJsonLoadedEventArgs { ObjectCount = 42 };

        Assert.That(args.ObjectCount, Is.EqualTo(42));
    }
}
