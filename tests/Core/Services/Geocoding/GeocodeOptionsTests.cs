using HerePlatformComponents.Maps.Services.Geocoding;

namespace HerePlatformComponents.Tests.Services.Geocoding;

[TestFixture]
public class GeocodeOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new GeocodeOptions();

        Assert.That(opts.Lang, Is.Null);
        Assert.That(opts.Limit, Is.EqualTo(5));
        Assert.That(opts.BoundingBox, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var opts = new GeocodeOptions
        {
            Lang = "de",
            Limit = 10,
            BoundingBox = "52.3,13.0,52.7,13.8"
        };

        Assert.That(opts.Lang, Is.EqualTo("de"));
        Assert.That(opts.Limit, Is.EqualTo(10));
        Assert.That(opts.BoundingBox, Is.EqualTo("52.3,13.0,52.7,13.8"));
    }
}
