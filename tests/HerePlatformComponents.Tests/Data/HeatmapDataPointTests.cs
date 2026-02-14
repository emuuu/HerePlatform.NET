using HerePlatformComponents.Maps.Data;

namespace HerePlatformComponents.Tests.Data;

[TestFixture]
public class HeatmapDataPointTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var point = new HeatmapDataPoint();

        Assert.That(point.Lat, Is.EqualTo(0));
        Assert.That(point.Lng, Is.EqualTo(0));
        Assert.That(point.Value, Is.EqualTo(1));
    }

    [Test]
    public void Constructor_SetsProperties()
    {
        var point = new HeatmapDataPoint(52.52, 13.405, 0.75);

        Assert.That(point.Lat, Is.EqualTo(52.52));
        Assert.That(point.Lng, Is.EqualTo(13.405));
        Assert.That(point.Value, Is.EqualTo(0.75));
    }

    [Test]
    public void Properties_AreSettable()
    {
        var point = new HeatmapDataPoint
        {
            Lat = 48.8566,
            Lng = 2.3522,
            Value = 0.5
        };

        Assert.That(point.Lat, Is.EqualTo(48.8566));
        Assert.That(point.Lng, Is.EqualTo(2.3522));
        Assert.That(point.Value, Is.EqualTo(0.5));
    }
}
