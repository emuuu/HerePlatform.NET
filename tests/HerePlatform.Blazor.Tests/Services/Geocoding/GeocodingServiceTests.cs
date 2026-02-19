using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Geocoding;
using HerePlatform.Core.Services;
using HerePlatform.Blazor.Maps;
using HerePlatform.Blazor.Maps.Services;
using Microsoft.JSInterop;

namespace HerePlatform.Blazor.Tests.Services.Geocoding;

[TestFixture]
public class GeocodingServiceTests : ServiceTestBase
{
    [Test]
    public async Task GeocodeAsync_WithResults_ReturnsItems()
    {
        MockJsResult("herePlatform.objectManager.geocode", new GeocodeResult
        {
            Items = new List<GeocodeItem>
            {
                new()
                {
                    Title = "Berlin Hauptbahnhof",
                    Position = new LatLngLiteral(52.5251, 13.3694),
                    Address = "Europaplatz 1, 10557 Berlin, Germany",
                    ResultType = "place"
                },
                new()
                {
                    Title = "Berlin Alexanderplatz",
                    Position = new LatLngLiteral(52.5219, 13.4132),
                    Address = "Alexanderplatz, 10178 Berlin, Germany",
                    ResultType = "locality"
                }
            }
        });
        var service = new GeocodingService(JsRuntime);

        var result = await service.GeocodeAsync("Berlin");

        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.Items![0].Title, Is.EqualTo("Berlin Hauptbahnhof"));
        Assert.That(result.Items[0].Position!.Value.Lat, Is.EqualTo(52.5251));
        Assert.That(result.Items[0].Position!.Value.Lng, Is.EqualTo(13.3694));
        Assert.That(result.Items[0].Address, Is.EqualTo("Europaplatz 1, 10557 Berlin, Germany"));
        Assert.That(result.Items[0].ResultType, Is.EqualTo("place"));
        Assert.That(result.Items[1].Title, Is.EqualTo("Berlin Alexanderplatz"));
        Assert.That(result.Items[1].ResultType, Is.EqualTo("locality"));
    }

    [Test]
    public async Task ReverseGeocodeAsync_WithResult_ReturnsItem()
    {
        MockJsResult("herePlatform.objectManager.reverseGeocode", new GeocodeResult
        {
            Items = new List<GeocodeItem>
            {
                new()
                {
                    Title = "Invalidenstraße 116",
                    Position = new LatLngLiteral(52.5310, 13.3847),
                    Address = "Invalidenstraße 116, 10115 Berlin, Germany",
                    ResultType = "houseNumber"
                }
            }
        });
        var service = new GeocodingService(JsRuntime);

        var result = await service.ReverseGeocodeAsync(new LatLngLiteral(52.5310, 13.3847));

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items![0].Address, Does.Contain("Invalidenstraße"));
        Assert.That(result.Items[0].Position, Is.Not.Null);
    }

    [Test]
    public async Task GeocodeAsync_NullResult_ReturnsEmptyResult()
    {
        // Loose mode returns default(GeocodeResult) = null for unmatched calls
        // Service does: result ?? new GeocodeResult()
        var service = new GeocodingService(JsRuntime);

        var result = await service.GeocodeAsync("nonexistent_place_xyz");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Is.Null);
    }

    [Test]
    public void GeocodeAsync_AuthError_ThrowsHereApiAuthenticationException()
    {
        MockJsException<GeocodeResult>(
            "herePlatform.objectManager.geocode",
            new JSException("Error: HERE_AUTH_ERROR:geocoding:HTTP 401"));
        var service = new GeocodingService(JsRuntime);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(async () =>
            await service.GeocodeAsync("Berlin"));

        Assert.That(ex!.Service, Is.EqualTo("geocoding"));
    }

    [Test]
    public void ReverseGeocodeAsync_AuthError_ThrowsHereApiAuthenticationException()
    {
        MockJsException<GeocodeResult>(
            "herePlatform.objectManager.reverseGeocode",
            new JSException("Error: HERE_AUTH_ERROR:reverse-geocoding:HTTP 403"));
        var service = new GeocodingService(JsRuntime);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(async () =>
            await service.ReverseGeocodeAsync(new LatLngLiteral(52.5310, 13.3847)));

        Assert.That(ex!.Service, Is.EqualTo("reverse-geocoding"));
    }
}
