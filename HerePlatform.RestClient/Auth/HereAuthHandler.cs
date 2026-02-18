using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace HerePlatform.RestClient.Auth;

internal sealed class HereAuthHandler : DelegatingHandler
{
    private readonly HereRestClientOptions _options;
    private readonly HereOAuthTokenManager? _oauthManager;

    public HereAuthHandler(IOptions<HereRestClientOptions> options, IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;

        if (!string.IsNullOrWhiteSpace(_options.AccessKeyId) && !string.IsNullOrWhiteSpace(_options.AccessKeySecret))
        {
            // Use a separate plain HttpClient for token requests (no auth handler to avoid recursion)
            var tokenClient = httpClientFactory.CreateClient("HereOAuthToken");
            _oauthManager = new HereOAuthTokenManager(_options.AccessKeyId, _options.AccessKeySecret, tokenClient);
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            // API Key: append as query parameter
            var uri = request.RequestUri!;
            var separator = string.IsNullOrEmpty(uri.Query) ? "?" : "&";
            request.RequestUri = new Uri($"{uri}{separator}apiKey={Uri.EscapeDataString(_options.ApiKey)}");
        }
        else if (_oauthManager is not null)
        {
            // OAuth 2.0: get cached/fresh token, set Bearer header
            var token = await _oauthManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else if (_options.TokenProvider is not null)
        {
            // Identity Provider: call external callback, set Bearer header
            var token = await _options.TokenProvider(cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _oauthManager?.Dispose();
        }
        base.Dispose(disposing);
    }
}
