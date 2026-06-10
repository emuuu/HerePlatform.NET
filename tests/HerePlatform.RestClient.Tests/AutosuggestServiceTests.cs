using System.Net;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Search;
using HerePlatform.RestClient.Services;

namespace HerePlatform.RestClient.Tests;

[TestFixture]
public class AutosuggestServiceTests
{
    private static RestAutosuggestService CreateService(MockHttpHandler handler)
    {
        var factory = new TestHttpClientFactory(handler);
        return new RestAutosuggestService(factory);
    }

    [Test]
    public async Task SuggestAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.SuggestAsync("Berlin", new AutosuggestOptions { Lang = "de", Limit = 3, In = "countryCode:DEU" });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://autosuggest.search.hereapi.com/v1/autosuggest?"));
        Assert.That(url, Does.Contain("q=Berlin"));
        Assert.That(url, Does.Contain("lang=de"));
        Assert.That(url, Does.Contain("limit=3"));
        Assert.That(url, Does.Contain("in=countryCode%3ADEU"));
    }

    [Test]
    public async Task SuggestAsync_WithAtParam_IncludesCoordinates()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.SuggestAsync("Berlin", new AutosuggestOptions { At = new LatLngLiteral(52.5, 13.4) });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("at=52.5%2C13.4"));
    }

    [Test]
    public async Task SuggestAsync_NullOptions_UsesDefaults()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.SuggestAsync("Berlin");

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("limit=5"));
        Assert.That(url, Does.Contain("lang=de"));
        Assert.That(url, Does.Contain("in=countryCode%3ADEU"));
        // in=countryCode without at is rejected by the HERE API (HTTP 400) —
        // the default options must always produce a request with a spatial context.
        Assert.That(url, Does.Contain("at=51.1657%2C10.4515"));
    }

    [Test]
    public void SuggestAsync_CountryCodeOnlyWithoutAt_ThrowsInsteadOfSending()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SuggestAsync("Berlin", new AutosuggestOptions { In = "countryCode:DEU", At = null }));

        Assert.That(ex!.Message, Does.Contain("spatial context"));
        Assert.That(handler.LastRequest, Is.Null);
    }

    [Test]
    public async Task SuggestAsync_InCircle_OmitsAt()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        // at and in=circle/bbox are mutually exclusive per the HERE API —
        // the default At must not leak into a request that already has a spatial context.
        await service.SuggestAsync("Berlin", new AutosuggestOptions { In = "circle:52.5,13.4;r=10000" });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("in=circle%3A52.5%2C13.4%3Br%3D10000"));
        Assert.That(url, Does.Not.Contain("at="));
    }

    [Test]
    public async Task SuggestAsync_MapsResponseCorrectly()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Berlin, Deutschland",
                    "id": "here:pds:place:276u33db-8097f3244e4b411081b761ea9a366776",
                    "resultType": "locality",
                    "address": {"label": "Berlin, Deutschland"},
                    "position": {"lat": 52.51604, "lng": 13.37691},
                    "highlights": {
                        "title": [{"start": 0, "end": 6}],
                        "address": [{"start": 0, "end": 6}]
                    }
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SuggestAsync("Berlin");

        Assert.That(result.Items, Has.Count.EqualTo(1));
        var item = result.Items![0];
        Assert.That(item.Title, Is.EqualTo("Berlin, Deutschland"));
        Assert.That(item.Id, Is.EqualTo("here:pds:place:276u33db-8097f3244e4b411081b761ea9a366776"));
        Assert.That(item.ResultType, Is.EqualTo("locality"));
        Assert.That(item.Address!.Label, Is.EqualTo("Berlin, Deutschland"));
        Assert.That(item.Position!.Value.Lat, Is.EqualTo(52.51604));
        Assert.That(item.Position!.Value.Lng, Is.EqualTo(13.37691));
    }

    [Test]
    public async Task SuggestAsync_MapsAllAddressFields()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Friedrichstr. 42, 10117 Berlin",
                    "resultType": "houseNumber",
                    "address": {
                        "label": "Friedrichstr. 42, 10117 Berlin, Deutschland",
                        "countryCode": "DEU",
                        "countryName": "Deutschland",
                        "state": "Berlin",
                        "city": "Berlin",
                        "district": "Mitte",
                        "street": "Friedrichstraße",
                        "postalCode": "10117",
                        "houseNumber": "42"
                    },
                    "position": {"lat": 52.5200, "lng": 13.3880}
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SuggestAsync("Friedrichstr");

        var addr = result.Items![0].Address!;
        Assert.That(addr.Label, Is.EqualTo("Friedrichstr. 42, 10117 Berlin, Deutschland"));
        Assert.That(addr.CountryCode, Is.EqualTo("DEU"));
        Assert.That(addr.CountryName, Is.EqualTo("Deutschland"));
        Assert.That(addr.State, Is.EqualTo("Berlin"));
        Assert.That(addr.City, Is.EqualTo("Berlin"));
        Assert.That(addr.District, Is.EqualTo("Mitte"));
        Assert.That(addr.Street, Is.EqualTo("Friedrichstraße"));
        Assert.That(addr.PostalCode, Is.EqualTo("10117"));
        Assert.That(addr.HouseNumber, Is.EqualTo("42"));
    }

    [Test]
    public async Task SuggestAsync_NullPosition_ForCategoryQuery()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Restaurants",
                    "resultType": "categoryQuery"
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SuggestAsync("Restaurant");

        Assert.That(result.Items![0].Position, Is.Null);
    }

    [Test]
    public async Task SuggestAsync_MapsHighlightsCorrectly()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Berlin",
                    "highlights": {
                        "title": [{"start": 0, "end": 3}],
                        "address": [{"start": 0, "end": 3}, {"start": 5, "end": 8}]
                    }
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SuggestAsync("Ber");

        var highlights = result.Items![0].Highlights!;
        Assert.That(highlights.Title, Has.Length.EqualTo(1));
        Assert.That(highlights.Title![0].Start, Is.EqualTo(0));
        Assert.That(highlights.Title[0].End, Is.EqualTo(3));
        Assert.That(highlights.Address, Has.Length.EqualTo(2));
        Assert.That(highlights.Address![1].Start, Is.EqualTo(5));
    }

    [Test]
    public async Task SuggestAsync_EmptyResponse_ReturnsEmptyItems()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        var result = await service.SuggestAsync("zzzznonexistent");

        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items, Is.Empty);
    }

    [Test]
    public void SuggestAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.SuggestAsync("Berlin"));
        Assert.That(ex!.Service, Is.EqualTo("autosuggest"));
    }

    // ── Autocomplete tests ──

    [Test]
    public async Task AutocompleteAsync_BuildsCorrectUrl()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.AutocompleteAsync("Berli", new AutosuggestOptions { Lang = "de", Limit = 5, In = "countryCode:DEU" });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.StartWith("https://autocomplete.search.hereapi.com/v1/autocomplete?"));
        Assert.That(url, Does.Contain("q=Berli"));
        Assert.That(url, Does.Contain("lang=de"));
        Assert.That(url, Does.Contain("limit=5"));
        Assert.That(url, Does.Contain("in=countryCode%3ADEU"));
    }

    [Test]
    public async Task AutocompleteAsync_MapsToAutosuggestItem()
    {
        var json = """
        {
            "items": [
                {
                    "title": "Berlin, Deutschland",
                    "id": "here:pds:place:276u33db-8097f3244e4b411081b761ea9a366776",
                    "resultType": "locality",
                    "address": {"label": "Berlin, Deutschland"},
                    "highlights": {
                        "title": [{"start": 0, "end": 5}]
                    }
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.AutocompleteAsync("Berli");

        Assert.That(result.Items, Has.Count.EqualTo(1));
        var item = result.Items![0];
        Assert.That(item, Is.TypeOf<AutosuggestItem>());
        Assert.That(item.Title, Is.EqualTo("Berlin, Deutschland"));
        Assert.That(item.Id, Is.EqualTo("here:pds:place:276u33db-8097f3244e4b411081b761ea9a366776"));
        Assert.That(item.ResultType, Is.EqualTo("locality"));
        Assert.That(item.Address!.Label, Is.EqualTo("Berlin, Deutschland"));
        Assert.That(item.Position, Is.Null);
        Assert.That(item.Highlights!.Title, Has.Length.EqualTo(1));
        Assert.That(item.Highlights.Title![0].End, Is.EqualTo(5));
    }

    [Test]
    public async Task AutocompleteAsync_EmptyResponse_ReturnsEmptyItems()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        var result = await service.AutocompleteAsync("zzzznonexistent");

        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.Items, Is.Empty);
    }

    [Test]
    public async Task AutocompleteAsync_CountryCodeOnlyWithoutAt_IsNotValidated()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        // The Autocomplete API accepts a standalone countryCode filter —
        // the autosuggest validation must not apply here.
        await service.AutocompleteAsync("Berli", new AutosuggestOptions { In = "countryCode:DEU", At = null });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("in=countryCode%3ADEU"));
        Assert.That(url, Does.Not.Contain("at="));
    }

    [Test]
    public async Task AutocompleteAsync_DefaultOptions_SendAtAsRankingBias()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.AutocompleteAsync("Berli");

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("in=countryCode%3ADEU"));
        Assert.That(url, Does.Contain("at=51.1657%2C10.4515"));
    }

    [Test]
    public void AutocompleteAsync_401_ThrowsAuthException()
    {
        var handler = MockHttpHandler.WithStatus(HttpStatusCode.Unauthorized);
        var service = CreateService(handler);

        var ex = Assert.ThrowsAsync<HereApiAuthenticationException>(
            () => service.AutocompleteAsync("Berlin"));
        Assert.That(ex!.Service, Is.EqualTo("autocomplete"));
    }
}
