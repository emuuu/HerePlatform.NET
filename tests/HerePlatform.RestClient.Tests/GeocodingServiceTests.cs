using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Geocoding;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class GeocodingServiceTests
{
    private static RestGeocodingService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestGeocodingService(factory);
    }

    [Test]
    public async Task GeocodeAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.GeocodeAsync("Berlin", new GeocodeOptions { Lang = "de", Limit = 3 });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://geocode.search.hereapi.com/v1/geocode?"));
        Assert.That(url, Does.Contain("q=Berlin"));
        Assert.That(url, Does.Contain("lang=de"));
        Assert.That(url, Does.Contain("limit=3"));
    }

    [Test]
    public async Task GeocodeAsync_WithBoundingBox_AddsBboxParam()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.GeocodeAsync("Berlin", new GeocodeOptions { BoundingBox = "52.3,13.0,52.7,13.8" });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("in=bbox%3A52.3%2C13.0%2C52.7%2C13.8"));
    }

    [Test]
    public async Task GeocodeAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Berlin, Deutschland",
                    "position": {"lat": 52.51604, "lng": 13.37691},
                    "address": {"label": "Berlin, Deutschland"},
                    "resultType": "locality"
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GeocodeAsync("Berlin");

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items![0].Title, Is.EqualTo("Berlin, Deutschland"));
        Assert.That(result.Items[0].Address, Is.EqualTo("Berlin, Deutschland"));
        Assert.That(result.Items[0].ResultType, Is.EqualTo("locality"));
        Assert.That(result.Items[0].Position, Is.Not.Null);
        Assert.That(result.Items[0].Position!.Value.Lat, Is.EqualTo(52.51604));
        Assert.That(result.Items[0].Position!.Value.Lng, Is.EqualTo(13.37691));
    }

    [Test]
    public async Task GeocodeAsync_EmptyResponse_ReturnsEmptyItems()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        var result = await service.GeocodeAsync("zzzznonexistent");

        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items, Is.Empty);
    }

    [Test]
    public void GeocodeAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.GeocodeAsync("Berlin"));
        Assert.That(ex!.Service, Is.EqualTo("geocoding"));
    }

    [Test]
    public void GeocodeAsync_400_ThrowsHereApiException()
    {
        var handler = MockHttpHandler.WithJson("""{"status":400,"title":"Bad Request"}""", HttpStatusCode.BadRequest);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiException>(
            () => service.GeocodeAsync("Berlin"));
        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.Service, Is.EqualTo("geocoding"));
        Assert.That(ex.ErrorBody, Does.Contain("Bad Request"));
    }

    [Test]
    public void GeocodeAsync_NullQuery_ThrowsArgumentNullException()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        Assert.ThrowsAsync<ArgumentNullException>(() => service.GeocodeAsync(null!));
    }

    [Test]
    public void GeocodeAsync_EmptyQuery_ThrowsArgumentException()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        Assert.ThrowsAsync<ArgumentException>(() => service.GeocodeAsync(""));
    }

    [Test]
    public async Task ReverseGeocodeAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.ReverseGeocodeAsync(new LatLngLiteral(52.51604, 13.37691));

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://revgeocode.search.hereapi.com/v1/revgeocode?"));
        Assert.That(url, Does.Contain("at=52.51604%2C13.37691"));
    }

    [Test]
    public async Task ReverseGeocodeAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Pariser Platz 1, 10117 Berlin",
                    "position": {"lat": 52.51604, "lng": 13.37691},
                    "address": {"label": "Pariser Platz 1, 10117 Berlin, Deutschland"},
                    "resultType": "houseNumber"
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.ReverseGeocodeAsync(new LatLngLiteral(52.51604, 13.37691));

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items![0].Title, Is.EqualTo("Pariser Platz 1, 10117 Berlin"));
    }

    [Test]
    public async Task ReverseGeocodeAsync_WithFullAddress_MapsAllAddressDetailsFields()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Falkensteinstraße 28, 46047 Oberhausen",
                    "position": {"lat": 51.4696, "lng": 6.8344},
                    "address": {
                        "label": "Falkensteinstraße 28, 46047 Oberhausen, Deutschland",
                        "countryCode": "DEU",
                        "countryName": "Deutschland",
                        "stateCode": "NW",
                        "state": "Nordrhein-Westfalen",
                        "countyCode": "OB",
                        "county": "Oberhausen",
                        "city": "Oberhausen",
                        "district": "Alstaden",
                        "street": "Falkensteinstraße",
                        "postalCode": "46047",
                        "houseNumber": "28"
                    },
                    "resultType": "houseNumber"
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.ReverseGeocodeAsync(new LatLngLiteral(51.4696, 6.8344));

        var item = result.Items![0];
        Assert.That(item.Address, Is.EqualTo("Falkensteinstraße 28, 46047 Oberhausen, Deutschland"));
        Assert.That(item.AddressDetails, Is.Not.Null);
        Assert.That(item.AddressDetails!.Label, Is.EqualTo("Falkensteinstraße 28, 46047 Oberhausen, Deutschland"));
        Assert.That(item.AddressDetails.CountryCode, Is.EqualTo("DEU"));
        Assert.That(item.AddressDetails.CountryName, Is.EqualTo("Deutschland"));
        Assert.That(item.AddressDetails.State, Is.EqualTo("Nordrhein-Westfalen"));
        Assert.That(item.AddressDetails.StateCode, Is.EqualTo("NW"));
        Assert.That(item.AddressDetails.County, Is.EqualTo("Oberhausen"));
        Assert.That(item.AddressDetails.CountyCode, Is.EqualTo("OB"));
        Assert.That(item.AddressDetails.City, Is.EqualTo("Oberhausen"));
        Assert.That(item.AddressDetails.District, Is.EqualTo("Alstaden"));
        Assert.That(item.AddressDetails.Street, Is.EqualTo("Falkensteinstraße"));
        Assert.That(item.AddressDetails.PostalCode, Is.EqualTo("46047"));
        Assert.That(item.AddressDetails.HouseNumber, Is.EqualTo("28"));
    }

    [Test]
    public async Task GeocodeAsync_WithoutAddress_AddressDetailsIsNull()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Berlin, Deutschland",
                    "position": {"lat": 52.51604, "lng": 13.37691},
                    "resultType": "locality"
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.GeocodeAsync("Berlin");

        Assert.That(result.Items![0].AddressDetails, Is.Null);
    }
}
