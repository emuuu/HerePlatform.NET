using HerePlatformComponents;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Search;

namespace HerePlatformComponents.Tests.Search;

[TestFixture]
public class AutosuggestOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var options = new AutosuggestOptions();

        Assert.That(options.Limit, Is.EqualTo(5));
        Assert.That(options.Lang, Is.EqualTo("de"));
        Assert.That(options.In, Is.EqualTo("countryCode:DEU"));
        Assert.That(options.At, Is.Null);
    }

    [Test]
    public void Serialize_DefaultOptions_ProducesCamelCaseJson()
    {
        var options = new AutosuggestOptions();
        var json = Helper.SerializeObject(options);

        Assert.That(json, Does.Contain("\"limit\":5"));
        Assert.That(json, Does.Contain("\"lang\":\"de\""));
        Assert.That(json, Does.Contain("\"in\":\"countryCode:DEU\""));
        Assert.That(json, Does.Not.Contain("\"at\""));
    }

    [Test]
    public void Serialize_WithAtPosition_IncludesPosition()
    {
        var options = new AutosuggestOptions
        {
            At = new LatLngLiteral(52.52, 13.405)
        };

        var json = Helper.SerializeObject(options);

        Assert.That(json, Does.Contain("\"lat\":52.52"));
        Assert.That(json, Does.Contain("\"lng\":13.405"));
    }

    [Test]
    public void Serialize_CustomValues()
    {
        var options = new AutosuggestOptions
        {
            Limit = 10,
            Lang = "en",
            In = "countryCode:USA"
        };

        var json = Helper.SerializeObject(options);

        Assert.That(json, Does.Contain("\"limit\":10"));
        Assert.That(json, Does.Contain("\"lang\":\"en\""));
        Assert.That(json, Does.Contain("\"in\":\"countryCode:USA\""));
    }

    [Test]
    public void Deserialize_FromJson()
    {
        var json = """{"limit":8,"lang":"fr","in":"countryCode:FRA","at":{"lat":48.8566,"lng":2.3522}}""";

        var options = Helper.DeSerializeObject<AutosuggestOptions>(json);

        Assert.That(options, Is.Not.Null);
        Assert.That(options!.Limit, Is.EqualTo(8));
        Assert.That(options.Lang, Is.EqualTo("fr"));
        Assert.That(options.In, Is.EqualTo("countryCode:FRA"));
        Assert.That(options.At, Is.Not.Null);
        Assert.That(options.At!.Value.Lat, Is.EqualTo(48.8566));
        Assert.That(options.At!.Value.Lng, Is.EqualTo(2.3522));
    }

    [Test]
    public void RoundTrip_PreservesValues()
    {
        var original = new AutosuggestOptions
        {
            Limit = 7,
            Lang = "it",
            In = "countryCode:ITA",
            At = new LatLngLiteral(41.9028, 12.4964)
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<AutosuggestOptions>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Limit, Is.EqualTo(original.Limit));
        Assert.That(result.Lang, Is.EqualTo(original.Lang));
        Assert.That(result.In, Is.EqualTo(original.In));
        Assert.That(result.At, Is.EqualTo(original.At));
    }
}
