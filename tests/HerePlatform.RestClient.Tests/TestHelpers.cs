using System.Net;

namespace HerePlatform.RestClient.Tests;

/// <summary>
/// Simple IHttpClientFactory that always returns an HttpClient backed by the given handler.
/// </summary>
internal class TestHttpClientFactory : IHttpClientFactory
{
    private readonly HttpMessageHandler _handler;

    public TestHttpClientFactory(HttpMessageHandler handler)
    {
        _handler = handler;
    }

    public HttpClient CreateClient(string name)
    {
        return new HttpClient(_handler);
    }
}
