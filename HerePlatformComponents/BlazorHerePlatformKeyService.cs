using System;
using System.Threading.Tasks;

namespace HerePlatformComponents;

public interface IBlazorHerePlatformKeyService
{
    Task<Maps.HereApiLoadOptions> GetApiOptions();
    bool IsApiInitialized { get; }
    void MarkApiInitialized();
    void UpdateApiKey(string apiKey);
}

public class BlazorHerePlatformKeyService : IBlazorHerePlatformKeyService
{
    private Maps.HereApiLoadOptions _initOptions;
    public bool IsApiInitialized { get; private set; }

    public void MarkApiInitialized() => IsApiInitialized = true;

    public BlazorHerePlatformKeyService(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        _initOptions = new Maps.HereApiLoadOptions(apiKey);
    }

    public BlazorHerePlatformKeyService(Maps.HereApiLoadOptions opts)
    {
        ArgumentNullException.ThrowIfNull(opts);
        _initOptions = opts;
    }

    public Task<Maps.HereApiLoadOptions> GetApiOptions()
    {
        return Task.FromResult(_initOptions);
    }

    public void UpdateApiKey(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        _initOptions = new Maps.HereApiLoadOptions(apiKey);
        IsApiInitialized = false;
    }
}
