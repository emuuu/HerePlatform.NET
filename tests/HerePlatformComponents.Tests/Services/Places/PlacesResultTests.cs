using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.Places;

namespace HerePlatformComponents.Tests.Services.Places;

[TestFixture]
public class PlacesResultTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var result = new PlacesResult();

        Assert.That(result.Items, Is.Null);
    }

    [Test]
    public void WithItems()
    {
        var result = new PlacesResult
        {
            Items = new List<PlaceItem>
            {
                new PlaceItem
                {
                    Title = "Brandenburg Gate",
                    Position = new LatLngLiteral(52.5163, 13.3777),
                    Address = "Pariser Platz, 10117 Berlin",
                    Categories = new List<string> { "sights-museums" },
                    Distance = 500,
                    PlaceId = "here:pds:place:276u33db-12345"
                }
            }
        };

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].Title, Is.EqualTo("Brandenburg Gate"));
        Assert.That(result.Items[0].Position!.Value.Lat, Is.EqualTo(52.5163));
        Assert.That(result.Items[0].Address, Is.EqualTo("Pariser Platz, 10117 Berlin"));
        Assert.That(result.Items[0].Categories, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].Distance, Is.EqualTo(500));
        Assert.That(result.Items[0].PlaceId, Is.EqualTo("here:pds:place:276u33db-12345"));
    }

    [Test]
    public void PlaceItem_DefaultValues()
    {
        var item = new PlaceItem();

        Assert.That(item.Title, Is.Null);
        Assert.That(item.Position, Is.Null);
        Assert.That(item.Address, Is.Null);
        Assert.That(item.Categories, Is.Null);
        Assert.That(item.OpeningHours, Is.Null);
        Assert.That(item.Contacts, Is.Null);
        Assert.That(item.Distance, Is.Null);
        Assert.That(item.PlaceId, Is.Null);
    }

    [Test]
    public void PlaceItem_WithContacts()
    {
        var item = new PlaceItem
        {
            Title = "Test Restaurant",
            Contacts = new List<PlaceContact>
            {
                new PlaceContact { Type = "phone", Value = "+49 30 123456" },
                new PlaceContact { Type = "website", Value = "https://example.com" }
            },
            OpeningHours = "Mon-Fri 09:00-18:00"
        };

        Assert.That(item.Contacts, Has.Count.EqualTo(2));
        Assert.That(item.Contacts[0].Type, Is.EqualTo("phone"));
        Assert.That(item.Contacts[0].Value, Is.EqualTo("+49 30 123456"));
        Assert.That(item.OpeningHours, Is.EqualTo("Mon-Fri 09:00-18:00"));
    }

    [Test]
    public void PlaceContact_DefaultValues()
    {
        var contact = new PlaceContact();

        Assert.That(contact.Type, Is.Null);
        Assert.That(contact.Value, Is.Null);
    }
}
