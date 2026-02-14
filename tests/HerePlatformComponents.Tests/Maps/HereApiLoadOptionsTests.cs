using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class HereApiLoadOptionsTests
{
    [Test]
    public void Constructor_SetsApiKey()
    {
        var opts = new HereApiLoadOptions("my-api-key");

        Assert.That(opts.ApiKey, Is.EqualTo("my-api-key"));
    }

    [Test]
    public void DefaultValues_AreCorrect()
    {
        var opts = new HereApiLoadOptions("key");

        Assert.That(opts.Version, Is.EqualTo("3.1"));
        Assert.That(opts.BaseUrl, Is.Null);
        Assert.That(opts.LoadMapEvents, Is.True);
        Assert.That(opts.LoadUI, Is.True);
        Assert.That(opts.LoadClustering, Is.False);
        Assert.That(opts.LoadData, Is.False);
        Assert.That(opts.UseHarpEngine, Is.True);
        Assert.That(opts.Language, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var opts = new HereApiLoadOptions("key")
        {
            Version = "3.2",
            BaseUrl = "https://custom.api.here.com",
            LoadMapEvents = false,
            LoadUI = false,
            LoadClustering = true,
            LoadData = true,
            UseHarpEngine = false,
            Language = "de-DE"
        };

        Assert.That(opts.Version, Is.EqualTo("3.2"));
        Assert.That(opts.BaseUrl, Is.EqualTo("https://custom.api.here.com"));
        Assert.That(opts.LoadMapEvents, Is.False);
        Assert.That(opts.LoadUI, Is.False);
        Assert.That(opts.LoadClustering, Is.True);
        Assert.That(opts.LoadData, Is.True);
        Assert.That(opts.UseHarpEngine, Is.False);
        Assert.That(opts.Language, Is.EqualTo("de-DE"));
    }
}
