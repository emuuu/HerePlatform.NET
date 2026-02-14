using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services;
using HerePlatformComponents.Maps.Services.Places;

namespace HerePlatformComponents.Tests.Services.Places;

[TestFixture]
public class PlacesServiceIntegrationTests : ServiceTestBase
{
    [Test]
    public async Task DiscoverAsync_WithResults_ReturnsPlaces()
    {
        MockJsResult("blazorHerePlatform.objectManager.discoverPlaces", new PlacesResult
        {
            Items = new List<PlaceItem>
            {
                new()
                {
                    Title = "Mustafas Gemüse Kebap",
                    Position = new LatLngLiteral(52.4907, 13.3880),
                    Address = "Mehringdamm 32, 10961 Berlin",
                    Categories = new List<string> { "restaurant", "fast-food" },
                    Contacts = new List<PlaceContact> { new() { Type = "phone", Value = "+4930123456" } },
                    Distance = 150
                },
                new()
                {
                    Title = "Café Einstein",
                    Position = new LatLngLiteral(52.5063, 13.3539),
                    Address = "Kurfürstenstraße 58, 10785 Berlin",
                    Categories = new List<string> { "cafe" },
                    Distance = 420
                },
                new()
                {
                    Title = "Bar Tausend",
                    Position = new LatLngLiteral(52.5208, 13.3833),
                    Address = "Schiffbauerdamm 11, 10117 Berlin",
                    Categories = new List<string> { "bar", "nightlife" },
                    Distance = 890
                }
            }
        });
        var service = new PlacesService(JsRuntime);

        var result = await service.DiscoverAsync(new PlacesRequest
        {
            Query = "restaurants",
            At = new LatLngLiteral(52.52, 13.405)
        });

        Assert.That(result.Items, Has.Count.EqualTo(3));
        Assert.That(result.Items![0].Title, Is.EqualTo("Mustafas Gemüse Kebap"));
        Assert.That(result.Items[0].Categories, Has.Count.EqualTo(2));
        Assert.That(result.Items[0].Categories, Contains.Item("restaurant"));
        Assert.That(result.Items[0].Contacts, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].Contacts![0].Value, Is.EqualTo("+4930123456"));
        Assert.That(result.Items[0].Distance, Is.EqualTo(150));
    }

    [Test]
    public async Task BrowseAsync_WithCategoryFilter_ReturnsFilteredPlaces()
    {
        MockJsResult("blazorHerePlatform.objectManager.browsePlaces", new PlacesResult
        {
            Items = new List<PlaceItem>
            {
                new()
                {
                    Title = "REWE City",
                    Position = new LatLngLiteral(52.5230, 13.4010),
                    Categories = new List<string> { "grocery" },
                    Distance = 200
                },
                new()
                {
                    Title = "Edeka",
                    Position = new LatLngLiteral(52.5180, 13.4090),
                    Categories = new List<string> { "grocery" },
                    Distance = 550
                }
            }
        });
        var service = new PlacesService(JsRuntime);

        var result = await service.BrowseAsync(new PlacesRequest
        {
            At = new LatLngLiteral(52.52, 13.405),
            Categories = new List<string> { "grocery" }
        });

        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.Items![0].Title, Is.EqualTo("REWE City"));
        Assert.That(result.Items[1].Title, Is.EqualTo("Edeka"));
    }

    [Test]
    public async Task LookupAsync_WithPlaceId_ReturnsDetailedPlace()
    {
        MockJsResult("blazorHerePlatform.objectManager.lookupPlace", new PlacesResult
        {
            Items = new List<PlaceItem>
            {
                new()
                {
                    Title = "Berliner Fernsehturm",
                    PlaceId = "here:pds:place:276u33dc-b224b7b5824a4e7e",
                    Position = new LatLngLiteral(52.5208, 13.4094),
                    Address = "Panoramastraße 1A, 10178 Berlin",
                    OpeningHours = "Mo-Su 10:00-23:00",
                    Contacts = new List<PlaceContact>
                    {
                        new() { Type = "phone", Value = "+493024575875" },
                        new() { Type = "website", Value = "https://tv-turm.de" }
                    }
                }
            }
        });
        var service = new PlacesService(JsRuntime);

        var result = await service.LookupAsync(new PlacesRequest
        {
            Id = "here:pds:place:276u33dc-b224b7b5824a4e7e"
        });

        Assert.That(result.Items, Has.Count.EqualTo(1));
        var place = result.Items![0];
        Assert.That(place.PlaceId, Is.EqualTo("here:pds:place:276u33dc-b224b7b5824a4e7e"));
        Assert.That(place.OpeningHours, Is.Not.Null);
        Assert.That(place.Contacts, Has.Count.EqualTo(2));
        Assert.That(place.Contacts![0].Type, Is.EqualTo("phone"));
        Assert.That(place.Contacts[1].Type, Is.EqualTo("website"));
    }
}
