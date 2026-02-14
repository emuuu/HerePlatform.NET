using HerePlatformComponents;
using HerePlatformComponents.Maps;
using Microsoft.Extensions.DependencyInjection;

namespace HerePlatformComponents.Tests.Services;

[TestFixture]
public class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddBlazorHerePlatform_WithApiKey_RegistersService()
    {
        var services = new ServiceCollection();

        services.AddBlazorHerePlatform("test-key");

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetService<IBlazorHerePlatformKeyService>();

        Assert.That(service, Is.Not.Null);
        Assert.That(service, Is.InstanceOf<BlazorHerePlatformKeyService>());
    }

    [Test]
    public async Task AddBlazorHerePlatform_WithApiKey_ServiceReturnsCorrectKey()
    {
        var services = new ServiceCollection();
        services.AddBlazorHerePlatform("my-api-key");

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBlazorHerePlatformKeyService>();

        var opts = await service.GetApiOptions();
        Assert.That(opts.ApiKey, Is.EqualTo("my-api-key"));
    }

    [Test]
    public void AddBlazorHerePlatform_WithOptions_RegistersService()
    {
        var services = new ServiceCollection();
        var apiOpts = new HereApiLoadOptions("key") { Language = "de-DE" };

        services.AddBlazorHerePlatform(apiOpts);

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetService<IBlazorHerePlatformKeyService>();

        Assert.That(service, Is.Not.Null);
    }

    [Test]
    public async Task AddBlazorHerePlatform_WithOptions_PreservesAllSettings()
    {
        var services = new ServiceCollection();
        var apiOpts = new HereApiLoadOptions("key")
        {
            Language = "de-DE",
            UseHarpEngine = true,
            LoadClustering = true
        };

        services.AddBlazorHerePlatform(apiOpts);

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBlazorHerePlatformKeyService>();
        var result = await service.GetApiOptions();

        Assert.That(result.Language, Is.EqualTo("de-DE"));
        Assert.That(result.UseHarpEngine, Is.True);
        Assert.That(result.LoadClustering, Is.True);
    }

    [Test]
    public void AddBlazorHerePlatform_WithCustomService_RegistersService()
    {
        var services = new ServiceCollection();
        var customService = new BlazorHerePlatformKeyService("custom");

        services.AddBlazorHerePlatform(customService);

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetService<IBlazorHerePlatformKeyService>();

        Assert.That(service, Is.SameAs(customService));
    }

    [Test]
    public void AddBlazorHerePlatform_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddBlazorHerePlatform("key");

        Assert.That(result, Is.SameAs(services));
    }

    [Test]
    public void AddBlazorHerePlatform_RegistersAsScoped()
    {
        var services = new ServiceCollection();
        services.AddBlazorHerePlatform("key");

        var descriptor = services.Single(d => d.ServiceType == typeof(IBlazorHerePlatformKeyService));

        Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
    }

    [Test]
    public void AddBlazorHerePlatform_WithCustomService_RegistersAsScoped()
    {
        var services = new ServiceCollection();
        services.AddBlazorHerePlatform(new BlazorHerePlatformKeyService("key"));

        var descriptor = services.Single(d => d.ServiceType == typeof(IBlazorHerePlatformKeyService));

        Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
    }

    [Test]
    public void AddBlazorHerePlatform_NullApiKey_Throws()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => services.AddBlazorHerePlatform((string)null!));
    }

    [Test]
    public void AddBlazorHerePlatform_EmptyApiKey_Throws()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentException>(() => services.AddBlazorHerePlatform(""));
    }

    [Test]
    public void AddBlazorHerePlatform_WhitespaceApiKey_Throws()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentException>(() => services.AddBlazorHerePlatform("   "));
    }

    [Test]
    public void AddBlazorHerePlatform_NullOptions_Throws()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => services.AddBlazorHerePlatform((HereApiLoadOptions)null!));
    }

    [Test]
    public void AddBlazorHerePlatform_NullKeyService_Throws()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => services.AddBlazorHerePlatform((IBlazorHerePlatformKeyService)null!));
    }
}
