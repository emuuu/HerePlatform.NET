using System.Threading.Tasks;

namespace HerePlatformComponents;

public interface IBlazorHerePlatformKeyService
{
    Task<Maps.HereApiLoadOptions> GetApiOptions();
    bool IsApiInitialized { get; set; }
    void UpdateApiKey(string apiKey);
}

public class BlazorHerePlatformKeyService : IBlazorHerePlatformKeyService
{
    private Maps.HereApiLoadOptions _initOptions;
    public bool IsApiInitialized { get; set; } = false;

    public BlazorHerePlatformKeyService(string apiKey)
    {
        _initOptions = new Maps.HereApiLoadOptions(apiKey);
    }

    public BlazorHerePlatformKeyService(Maps.HereApiLoadOptions opts)
    {
        _initOptions = opts;
    }

    public Task<Maps.HereApiLoadOptions> GetApiOptions()
    {
        return Task.FromResult(_initOptions);
    }

    public void UpdateApiKey(string apiKey)
    {
        _initOptions = new Maps.HereApiLoadOptions(apiKey);
        IsApiInitialized = false;
    }
}
