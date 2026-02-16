using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.Places;

namespace HerePlatformComponents.Tests.Services.Places;

[TestFixture]
public class PlacesRequestTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var request = new PlacesRequest();

        Assert.That(request.Query, Is.Null);
        Assert.That(request.At, Is.Null);
        Assert.That(request.BoundingBox, Is.Null);
        Assert.That(request.Categories, Is.Null);
        Assert.That(request.Limit, Is.EqualTo(20));
        Assert.That(request.Lang, Is.Null);
        Assert.That(request.Id, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var request = new PlacesRequest
        {
            Query = "restaurants",
            At = new LatLngLiteral(52.52, 13.405),
            BoundingBox = "52.0,13.0,53.0,14.0",
            Categories = new List<string> { "100-1000-0000", "200-2000-0000" },
            Limit = 10,
            Lang = "de",
            Id = "here:pds:place:276u33db-12345"
        };

        Assert.That(request.Query, Is.EqualTo("restaurants"));
        Assert.That(request.At!.Value.Lat, Is.EqualTo(52.52));
        Assert.That(request.BoundingBox, Is.EqualTo("52.0,13.0,53.0,14.0"));
        Assert.That(request.Categories, Has.Count.EqualTo(2));
        Assert.That(request.Limit, Is.EqualTo(10));
        Assert.That(request.Lang, Is.EqualTo("de"));
        Assert.That(request.Id, Is.EqualTo("here:pds:place:276u33db-12345"));
    }
}
