using System.Net;

namespace HerePlatform.RestClient.Tests;

internal class MockHttpHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestBody { get; private set; }
    public List<HttpRequestMessage> AllRequests { get; } = [];

    public MockHttpHandler(HttpResponseMessage response)
        : this(_ => response)
    {
    }

    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        LastRequestBody = request.Content is not null
            ? await request.Content.ReadAsStringAsync(cancellationToken)
            : null;
        AllRequests.Add(request);
        return _handler(request);
    }

    public static MockHttpHandler WithJson(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new MockHttpHandler(new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });
    }

    public static MockHttpHandler WithStatus(HttpStatusCode statusCode)
    {
        return new MockHttpHandler(new HttpResponseMessage(statusCode));
    }

    public static MockHttpHandler WithBytes(byte[] bytes, string contentType)
    {
        return new MockHttpHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(bytes)
            {
                Headers = { { "Content-Type", contentType } }
            }
        });
    }
}
