using HerePlatformComponents;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Services;

[TestFixture]
public class BlazorHerePlatformKeyServiceTests
{
    [Test]
    public void Constructor_WithApiKey_CreatesOptions()
    {
        var service = new BlazorHerePlatformKeyService("my-key");

        Assert.That(service.IsApiInitialized, Is.False);
    }

    [Test]
    public async Task GetApiOptions_WithApiKey_ReturnsCorrectKey()
    {
        var service = new BlazorHerePlatformKeyService("my-key");

        var options = await service.GetApiOptions();

        Assert.That(options.ApiKey, Is.EqualTo("my-key"));
        Assert.That(options.Version, Is.EqualTo("3.1"));
    }

    [Test]
    public void Constructor_WithOptions_PreservesOptions()
    {
        var opts = new HereApiLoadOptions("custom-key")
        {
            Version = "3.2",
            Language = "de-DE",
            UseHarpEngine = true
        };

        var service = new BlazorHerePlatformKeyService(opts);

        Assert.That(service.IsApiInitialized, Is.False);
    }

    [Test]
    public async Task GetApiOptions_WithOptions_ReturnsSameInstance()
    {
        var opts = new HereApiLoadOptions("key") { Language = "en-US" };
        var service = new BlazorHerePlatformKeyService(opts);

        var result = await service.GetApiOptions();

        Assert.That(result, Is.SameAs(opts));
        Assert.That(result.Language, Is.EqualTo("en-US"));
    }

    [Test]
    public void IsApiInitialized_CanBeSet()
    {
        var service = new BlazorHerePlatformKeyService("key");

        service.IsApiInitialized = true;

        Assert.That(service.IsApiInitialized, Is.True);
    }

    [Test]
    public async Task GetApiOptions_CalledMultipleTimes_ReturnsSameInstance()
    {
        var service = new BlazorHerePlatformKeyService("key");

        var result1 = await service.GetApiOptions();
        var result2 = await service.GetApiOptions();

        Assert.That(result1, Is.SameAs(result2));
    }

    [Test]
    public async Task UpdateApiKey_ChangesApiKey()
    {
        var service = new BlazorHerePlatformKeyService("old-key");
        service.IsApiInitialized = true;

        service.UpdateApiKey("new-key");

        var options = await service.GetApiOptions();
        Assert.That(options.ApiKey, Is.EqualTo("new-key"));
    }

    [Test]
    public void UpdateApiKey_ResetsIsApiInitialized()
    {
        var service = new BlazorHerePlatformKeyService("key");
        service.IsApiInitialized = true;

        service.UpdateApiKey("new-key");

        Assert.That(service.IsApiInitialized, Is.False);
    }

    [Test]
    public async Task UpdateApiKey_CreatesNewOptionsInstance()
    {
        var service = new BlazorHerePlatformKeyService("key");
        var original = await service.GetApiOptions();

        service.UpdateApiKey("new-key");
        var updated = await service.GetApiOptions();

        Assert.That(updated, Is.Not.SameAs(original));
    }
}
