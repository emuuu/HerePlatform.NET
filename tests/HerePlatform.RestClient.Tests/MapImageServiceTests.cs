using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.MapImage;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class MapImageServiceTests
{
    private static RestMapImageService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestMapImageService(factory);
    }

    [Test]
    public async Task GetImageAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithBytes(new byte[] { 0x89, 0x50 }, "image/png");
        var service = CreateService(handler);

        await service.GetImageAsync(new MapImageRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Zoom = 14,
            Width = 800,
            Height = 600,
            Format = MapImageFormat.Png,
            Style = MapImageStyle.Default
        });

        var url = handler.LastRequest!.RequestUri!.OriginalString;
        Assert.That(url, Does.StartWith("https://image.maps.hereapi.com/mia/v3/base/mc/"));
        Assert.That(url, Does.Contain("center:52.5,13.4;zoom=14"));
        Assert.That(url, Does.Contain("800x600"));
        Assert.That(url, Does.Contain("/png?"));
        Assert.That(url, Does.Contain("style=explore.day"));
    }

    [Test]
    public async Task GetImageAsync_IncludesStyleParam()
    {
        var handler = MockHttpHandler.WithBytes([], "image/png");
        var service = CreateService(handler);

        await service.GetImageAsync(new MapImageRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Style = MapImageStyle.Night
        });

        var url = handler.LastRequest!.RequestUri!.OriginalString;
        Assert.That(url, Does.Contain("style=explore.night"));
    }

    [Test]
    public async Task GetImageAsync_IncludesPpiParam()
    {
        var handler = MockHttpHandler.WithBytes([], "image/png");
        var service = CreateService(handler);

        await service.GetImageAsync(new MapImageRequest
        {
            Center = new LatLngLiteral(52.5, 13.4),
            Ppi = 250
        });

        var url = handler.LastRequest!.RequestUri!.OriginalString;
        Assert.That(url, Does.Contain("ppi=250"));
    }

    [Test]
    public async Task GetImageAsync_ReturnsBinaryContent()
    {
        var expectedBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A };
        var handler = MockHttpHandler.WithBytes(expectedBytes, "image/png");
        var service = CreateService(handler);

        var result = await service.GetImageAsync(new MapImageRequest
        {
            Center = new LatLngLiteral(52.5, 13.4)
        });

        Assert.That(result, Is.EqualTo(expectedBytes));
    }

    [Test]
    public void GetImageAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.GetImageAsync(new MapImageRequest
            {
                Center = new LatLngLiteral(52.5, 13.4)
            }));
        Assert.That(ex!.Service, Is.EqualTo("mapImage"));
    }
}
