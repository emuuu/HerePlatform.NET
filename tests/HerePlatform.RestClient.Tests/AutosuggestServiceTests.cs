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
        // Default Show — without show=details the API returns only address.label.
        Assert.That(url, Does.Contain("show=details"));
    }

    [Test]
    public async Task SuggestAsync_ShowNull_OmitsShowParam()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.SuggestAsync("Berlin", new AutosuggestOptions { Show = null });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Not.Contain("show="));
    }

    [Test]
    public async Task SuggestAsync_ShowEmpty_OmitsShowParam()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        // Empty must behave like null ("show=" would be sent otherwise) —
        // consistent with the JS path, which drops falsy show values.
        await service.SuggestAsync("Berlin", new AutosuggestOptions { Show = " " });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Not.Contain("show="));
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
        // Response shape as returned with show=details (verified against the live API).
        var json = """
        {
            "items": [
                {
                    "title": "Falkensteinstraße 28, 46047 Oberhausen",
                    "resultType": "houseNumber",
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
                    "position": {"lat": 51.4696, "lng": 6.8344}
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SuggestAsync("Falkensteinstr");

        var addr = result.Items![0].Address!;
        Assert.That(addr.Label, Is.EqualTo("Falkensteinstraße 28, 46047 Oberhausen, Deutschland"));
        Assert.That(addr.CountryCode, Is.EqualTo("DEU"));
        Assert.That(addr.CountryName, Is.EqualTo("Deutschland"));
        Assert.That(addr.StateCode, Is.EqualTo("NW"));
        Assert.That(addr.State, Is.EqualTo("Nordrhein-Westfalen"));
        Assert.That(addr.CountyCode, Is.EqualTo("OB"));
        Assert.That(addr.County, Is.EqualTo("Oberhausen"));
        Assert.That(addr.City, Is.EqualTo("Oberhausen"));
        Assert.That(addr.District, Is.EqualTo("Alstaden"));
        Assert.That(addr.Street, Is.EqualTo("Falkensteinstraße"));
        Assert.That(addr.PostalCode, Is.EqualTo("46047"));
        Assert.That(addr.HouseNumber, Is.EqualTo("28"));
    }

    [Test]
    public async Task SuggestAsync_LabelOnlyResponse_LeavesStructuredFieldsNull()
    {
        // Response shape as returned WITHOUT show=details (verified against the
        // live API): only address.label is present, no structured fields.
        var json = """
        {
            "items": [
                {
                    "title": "Falkensteinstraße 28, 46047 Oberhausen",
                    "resultType": "houseNumber",
                    "address": {"label": "Falkensteinstraße 28, 46047 Oberhausen, Deutschland"},
                    "position": {"lat": 51.4696, "lng": 6.8344}
                }
            ]
        }
        """;
        var handler = MockHttpHandler.WithJson(json);
        var service = CreateService(handler);

        var result = await service.SuggestAsync("Falkensteinstr", new AutosuggestOptions { Show = null });

        var addr = result.Items![0].Address!;
        Assert.That(addr.Label, Is.EqualTo("Falkensteinstraße 28, 46047 Oberhausen, Deutschland"));
        Assert.That(addr.CountryCode, Is.Null);
        Assert.That(addr.State, Is.Null);
        Assert.That(addr.StateCode, Is.Null);
        Assert.That(addr.County, Is.Null);
        Assert.That(addr.CountyCode, Is.Null);
        Assert.That(addr.City, Is.Null);
        Assert.That(addr.District, Is.Null);
        Assert.That(addr.Street, Is.Null);
        Assert.That(addr.PostalCode, Is.Null);
        Assert.That(addr.HouseNumber, Is.Null);
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
        // The default Show ("details") must not leak into autocomplete requests —
        // /autocomplete rejects show=details (it only supports streetInfo/hasRelatedMPA).
        Assert.That(url, Does.Not.Contain("show="));
    }

    [Test]
    public async Task AutocompleteAsync_ExplicitShow_IsNeverSent()
    {
        var handler = MockHttpHandler.WithJson("""{"items":[]}""");
        var service = CreateService(handler);

        await service.AutocompleteAsync("Berli", new AutosuggestOptions { Show = "details" });

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Not.Contain("show="));
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
