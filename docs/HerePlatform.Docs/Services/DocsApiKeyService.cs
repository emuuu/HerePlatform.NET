namespace HerePlatform.Docs.Services;

public sealed class DocsApiKeyService
{
    private string _apiKey = "";

    public string ApiKey => _apiKey;
    public bool HasKey => !string.IsNullOrWhiteSpace(_apiKey);

    public event Action? ApiKeyChanged;

    public void UpdateApiKey(string key)
    {
        _apiKey = key?.Trim() ?? "";
        ApiKeyChanged?.Invoke();
    }
}
