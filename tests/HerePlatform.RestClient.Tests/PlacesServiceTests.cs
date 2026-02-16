using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Places;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class PlacesServiceTests
{
    private static RestPlacesService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestPlacesService(factory);
    }

    // --- Discover ---

    [Test]
    public async Task DiscoverAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.DiscoverAsync(new PlacesRequest
        {
            Query = "restaurant",
            At = new LatLngLiteral(52.5, 13.4),
            Limit = 10,
            Lang = "de"
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://discover.search.hereapi.com/v1/discover?"));
        Assert.That(url, Does.Contain("q=restaurant"));
        Assert.That(url, Does.Contain("at=52.5%2C13.4"));
        Assert.That(url, Does.Contain("limit=10"));
        Assert.That(url, Does.Contain("lang=de"));
    }

    [Test]
    public async Task DiscoverAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "items": [
                {
                    "id": "here:pds:place:276u0vhj-xxxx",
                    "title": "Best Restaurant",
                    "position": {"lat": 52.5, "lng": 13.4},
                    "address": {"label": "Friedrichstr. 1, 10117 Berlin"},
                    "categories": [{"name": "Restaurant"}, {"name": "Italian"}],
                    "openingHours": [{"text": ["Mo-Fr 11:00-22:00"]}],
                    "contacts": [{"phone": [{"value": "+491234567"}], "www": [{"value": "https://example.com"}]}],
                    "distance": 150
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.DiscoverAsync(new PlacesRequest { Query = "restaurant" });

        Assert.That(result.Items, Has.Count.EqualTo(1));
        var item = result.Items![0];
        Assert.That(item.PlaceId, Is.EqualTo("here:pds:place:276u0vhj-xxxx"));
        Assert.That(item.Title, Is.EqualTo("Best Restaurant"));
        Assert.That(item.Address, Is.EqualTo("Friedrichstr. 1, 10117 Berlin"));
        Assert.That(item.Position!.Value.Lat, Is.EqualTo(52.5));
        Assert.That(item.Categories, Has.Count.EqualTo(2));
        Assert.That(item.Categories![0], Is.EqualTo("Restaurant"));
        Assert.That(item.OpeningHours, Is.EqualTo("Mo-Fr 11:00-22:00"));
        Assert.That(item.Contacts, Has.Count.EqualTo(2));
        Assert.That(item.Contacts![0].Type, Is.EqualTo("phone"));
        Assert.That(item.Contacts[0].Value, Is.EqualTo("+491234567"));
        Assert.That(item.Contacts[1].Type, Is.EqualTo("website"));
        Assert.That(item.Distance, Is.EqualTo(150));
    }

    [Test]
    public void DiscoverAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.DiscoverAsync(new PlacesRequest { Query = "test" }));
        Assert.That(ex!.Service, Is.EqualTo("places"));
    }

    // --- Browse ---

    [Test]
    public async Task BrowseAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.BrowseAsync(new PlacesRequest
        {
            At = new LatLngLiteral(52.5, 13.4),
            Categories = ["100-1000-0000", "200-2000-0000"],
            Limit = 5
        });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://browse.search.hereapi.com/v1/browse?"));
        Assert.That(url, Does.Contain("categories=100-1000-0000%2C200-2000-0000"));
        Assert.That(url, Does.Contain("limit=5"));
    }

    // --- Lookup ---

    [Test]
    public async Task LookupAsync_BuildsCorrectUrl()
    {
        var json = """
        {
            "id": "here:pds:place:276u0vhj-xxxx",
            "title": "Test Place",
            "position": {"lat": 52.5, "lng": 13.4},
            "address": {"label": "Test Address"}
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        await service.LookupAsync(new PlacesRequest { Id = "here:pds:place:276u0vhj-xxxx", Lang = "de" });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://lookup.search.hereapi.com/v1/lookup?"));
        Assert.That(url, Does.Contain("id=here%3Apds%3Aplace%3A276u0vhj-xxxx"));
        Assert.That(url, Does.Contain("lang=de"));
    }

    [Test]
    public async Task LookupAsync_ReturnsSingleItemInResult()
    {
        var json = """
        {
            "id": "here:pds:place:abc",
            "title": "Lookup Place",
            "position": {"lat": 52.5, "lng": 13.4},
            "address": {"label": "Lookup Address"}
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.LookupAsync(new PlacesRequest { Id = "here:pds:place:abc" });

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items![0].PlaceId, Is.EqualTo("here:pds:place:abc"));
        Assert.That(result.Items[0].Title, Is.EqualTo("Lookup Place"));
    }

    [Test]
    public async Task DiscoverAsync_EmptyResponse_ReturnsEmptyItems()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        var result = await service.DiscoverAsync(new PlacesRequest { Query = "nonexistent" });

        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items, Is.Empty);
    }
}
