namespace HerePlatform.Docs.Services;

public sealed class DocsHttpClientFactory : IHttpClientFactory
{
    private readonly DocsApiKeyService _apiKeyService;

    public DocsHttpClientFactory(DocsApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    public HttpClient CreateClient(string name)
    {
        var handler = new ApiKeyInjectingHandler(_apiKeyService)
        {
            InnerHandler = new HttpClientHandler()
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        return client;
    }

    private sealed class ApiKeyInjectingHandler : DelegatingHandler
    {
        private readonly DocsApiKeyService _apiKeyService;

        public ApiKeyInjectingHandler(DocsApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_apiKeyService.HasKey)
            {
                var uri = request.RequestUri!;
                var separator = string.IsNullOrEmpty(uri.Query) ? "?" : "&";
                request.RequestUri = new Uri($"{uri}{separator}apiKey={Uri.EscapeDataString(_apiKeyService.ApiKey)}");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
