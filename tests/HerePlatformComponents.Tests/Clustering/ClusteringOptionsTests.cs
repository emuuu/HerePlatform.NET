using HerePlatformComponents.Maps.Clustering;

namespace HerePlatformComponents.Tests.Clustering;

[TestFixture]
public class ClusteringOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new ClusteringOptions();

        Assert.That(opts.Eps, Is.EqualTo(32));
        Assert.That(opts.MinWeight, Is.EqualTo(2));
        Assert.That(opts.Strategy, Is.EqualTo(ClusteringStrategy.FastGrid));
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var opts = new ClusteringOptions
        {
            Eps = 64,
            MinWeight = 5,
            Strategy = ClusteringStrategy.DynamicGrid
        };

        Assert.That(opts.Eps, Is.EqualTo(64));
        Assert.That(opts.MinWeight, Is.EqualTo(5));
        Assert.That(opts.Strategy, Is.EqualTo(ClusteringStrategy.DynamicGrid));
    }
}
