using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.Geocoding;

namespace HerePlatformComponents.Tests.Services.Geocoding;

[TestFixture]
public class GeocodeResultTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var result = new GeocodeResult();

        Assert.That(result.Items, Is.Null);
    }

    [Test]
    public void WithItems()
    {
        var result = new GeocodeResult
        {
            Items = new List<GeocodeItem>
            {
                new GeocodeItem
                {
                    Title = "Berlin",
                    Position = new LatLngLiteral(52.52, 13.405),
                    Address = "Berlin, Germany",
                    ResultType = "locality"
                }
            }
        };

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].Title, Is.EqualTo("Berlin"));
        Assert.That(result.Items[0].Position!.Value.Lat, Is.EqualTo(52.52));
        Assert.That(result.Items[0].Address, Is.EqualTo("Berlin, Germany"));
        Assert.That(result.Items[0].ResultType, Is.EqualTo("locality"));
    }

    [Test]
    public void GeocodeItem_DefaultValues()
    {
        var item = new GeocodeItem();

        Assert.That(item.Title, Is.Null);
        Assert.That(item.Position, Is.Null);
        Assert.That(item.Address, Is.Null);
        Assert.That(item.ResultType, Is.Null);
    }
}
