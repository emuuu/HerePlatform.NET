using System;

namespace HerePlatformComponents.Maps;

public class HereApiLoadOptions
{
    public string ApiKey { get; init; }
    public string Version { get; set; } = "3.1";
    public string? BaseUrl { get; set; }
    public bool LoadMapEvents { get; set; } = true;
    public bool LoadUI { get; set; } = true;
    public bool LoadClustering { get; set; } = false;
    public bool LoadData { get; set; } = false;
    public bool UseHarpEngine { get; set; } = true;
    public string? Language { get; set; }

    public HereApiLoadOptions(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ApiKey = apiKey;
    }
}
