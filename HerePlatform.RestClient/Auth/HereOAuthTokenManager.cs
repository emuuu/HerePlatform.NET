using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HerePlatform.RestClient.Auth;

internal sealed class HereOAuthTokenManager : IDisposable
{
    private const string TokenEndpoint = "https://account.api.here.com/oauth2/token";
    private static readonly TimeSpan RefreshMargin = TimeSpan.FromSeconds(60);

    private readonly string _accessKeyId;
    private readonly string _accessKeySecret;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private volatile CachedToken? _cached;

    private sealed record CachedToken(string Token, DateTimeOffset Expiry);

    public HereOAuthTokenManager(string accessKeyId, string accessKeySecret, HttpClient httpClient)
    {
        _accessKeyId = accessKeyId;
        _accessKeySecret = accessKeySecret;
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var snapshot = _cached;
        if (snapshot is not null && DateTimeOffset.UtcNow < snapshot.Expiry - RefreshMargin)
            return snapshot.Token;

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Double-check after acquiring lock
            snapshot = _cached;
            if (snapshot is not null && DateTimeOffset.UtcNow < snapshot.Expiry - RefreshMargin)
                return snapshot.Token;

            return await RequestTokenAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<string> RequestTokenAsync(CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString("N");

        var oauthParams = new SortedDictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["oauth_consumer_key"] = _accessKeyId,
            ["oauth_nonce"] = nonce,
            ["oauth_signature_method"] = "HMAC-SHA256",
            ["oauth_timestamp"] = timestamp,
            ["oauth_version"] = "1.0"
        };

        // Build signature base string: POST&url_encoded_endpoint&sorted_params
        var paramString = string.Join("&",
            oauthParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        var signatureBase = $"POST&{Uri.EscapeDataString(TokenEndpoint)}&{Uri.EscapeDataString(paramString)}";

        // Signing key: url_encoded_secret& (no token secret for HERE)
        var signingKey = $"{Uri.EscapeDataString(_accessKeySecret)}&";
        var signature = ComputeHmacSha256(signingKey, signatureBase);

        // Build Authorization header
        var authHeader = $"OAuth " +
            $"oauth_consumer_key=\"{Uri.EscapeDataString(_accessKeyId)}\","+
            $"oauth_nonce=\"{Uri.EscapeDataString(nonce)}\","+
            $"oauth_signature=\"{Uri.EscapeDataString(signature)}\","+
            $"oauth_signature_method=\"HMAC-SHA256\","+
            $"oauth_timestamp=\"{timestamp}\","+
            $"oauth_version=\"1.0\"";

        using var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            })
        };
        request.Headers.TryAddWithoutValidation("Authorization", authHeader);

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var token = root.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("OAuth response missing access_token.");
        var expiresIn = root.GetProperty("expires_in").GetInt32();
        var expiry = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

        _cached = new CachedToken(token, expiry);

        return token;
    }

    internal static string ComputeHmacSha256(string key, string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
