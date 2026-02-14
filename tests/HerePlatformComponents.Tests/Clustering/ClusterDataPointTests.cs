using HerePlatformComponents.Maps.Clustering;

namespace HerePlatformComponents.Tests.Clustering;

[TestFixture]
public class ClusterDataPointTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var point = new ClusterDataPoint();

        Assert.That(point.Lat, Is.EqualTo(0.0));
        Assert.That(point.Lng, Is.EqualTo(0.0));
        Assert.That(point.Weight, Is.EqualTo(1));
        Assert.That(point.Data, Is.Null);
    }

    [Test]
    public void Constructor_SetsAllProperties()
    {
        var data = new { Id = 42 };
        var point = new ClusterDataPoint(52.52, 13.405, 5, data);

        Assert.That(point.Lat, Is.EqualTo(52.52));
        Assert.That(point.Lng, Is.EqualTo(13.405));
        Assert.That(point.Weight, Is.EqualTo(5));
        Assert.That(point.Data, Is.SameAs(data));
    }

    [Test]
    public void Properties_AreSettable()
    {
        var point = new ClusterDataPoint
        {
            Lat = 48.8566,
            Lng = 2.3522,
            Weight = 3
        };

        Assert.That(point.Lat, Is.EqualTo(48.8566));
        Assert.That(point.Lng, Is.EqualTo(2.3522));
        Assert.That(point.Weight, Is.EqualTo(3));
    }

    [Test]
    public void Serialization_ProducesCorrectJson()
    {
        var point = new ClusterDataPoint(52.52, 13.405, 2);
        var json = HerePlatformComponents.Helper.SerializeObject(point);

        Assert.That(json, Does.Contain("\"lat\":52.52"));
        Assert.That(json, Does.Contain("\"lng\":13.405"));
        Assert.That(json, Does.Contain("\"weight\":2"));
    }
}
