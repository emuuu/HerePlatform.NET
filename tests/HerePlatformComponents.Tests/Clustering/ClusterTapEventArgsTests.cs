using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Clustering;

namespace HerePlatformComponents.Tests.Clustering;

[TestFixture]
public class ClusterTapEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new ClusterTapEventArgs();

        Assert.That(args.Position, Is.Null);
        Assert.That(args.Weight, Is.EqualTo(0));
        Assert.That(args.PointCount, Is.EqualTo(0));
        Assert.That(args.IsCluster, Is.False);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var args = new ClusterTapEventArgs
        {
            Position = new LatLngLiteral(52.52, 13.405),
            Weight = 15,
            PointCount = 15,
            IsCluster = true
        };

        Assert.That(args.Position!.Value.Lat, Is.EqualTo(52.52));
        Assert.That(args.Weight, Is.EqualTo(15));
        Assert.That(args.PointCount, Is.EqualTo(15));
        Assert.That(args.IsCluster, Is.True);
    }

    [Test]
    public void NoisePoint_HasCorrectValues()
    {
        var args = new ClusterTapEventArgs
        {
            Position = new LatLngLiteral(48.8566, 2.3522),
            Weight = 1,
            PointCount = 1,
            IsCluster = false
        };

        Assert.That(args.IsCluster, Is.False);
        Assert.That(args.Weight, Is.EqualTo(1));
    }
}
