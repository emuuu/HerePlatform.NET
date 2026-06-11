using HerePlatform.Core.Coordinates;
using HerePlatform.Blazor;
using HerePlatform.Blazor.Maps;
using HerePlatform.Core.Search;
using HerePlatform.Blazor.Maps.Search;

namespace HerePlatform.Blazor.Tests.Search;

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
        // The HERE API rejects in=countryCode without a spatial context (at/circle/bbox),
        // so At must default to a coordinate — the geographic center of Germany.
        Assert.That(options.At, Is.EqualTo(new LatLngLiteral(51.1657, 10.4515)));
        // Without show=details the API returns only address.label — the structured
        // AutosuggestAddress fields would silently stay null, so details is the default.
        Assert.That(options.Show, Is.EqualTo("details"));
    }

    [Test]
    public void Serialize_DefaultOptions_ProducesCamelCaseJson()
    {
        var options = new AutosuggestOptions();
        var json = Helper.SerializeObject(options);

        Assert.That(json, Does.Contain("\"limit\":5"));
        Assert.That(json, Does.Contain("\"lang\":\"de\""));
        Assert.That(json, Does.Contain("\"in\":\"countryCode:DEU\""));
        Assert.That(json, Does.Contain("\"show\":\"details\""));
        Assert.That(json, Does.Contain("\"at\""));
        Assert.That(json, Does.Contain("\"lat\":51.1657"));
        Assert.That(json, Does.Contain("\"lng\":10.4515"));
    }

    [Test]
    public void EnsureValidForAutosuggest_DefaultOptions_DoesNotThrow()
    {
        var options = new AutosuggestOptions();

        Assert.That(() => options.EnsureValidForAutosuggest(), Throws.Nothing);
    }

    [Test]
    public void EnsureValidForAutosuggest_CountryCodeOnlyWithoutAt_Throws()
    {
        var options = new AutosuggestOptions { In = "countryCode:DEU", At = null };

        var ex = Assert.Throws<InvalidOperationException>(() => options.EnsureValidForAutosuggest());
        Assert.That(ex!.Message, Does.Contain("spatial context"));
        Assert.That(ex.Message, Does.Contain("countryCode"));
    }

    [Test]
    public void EnsureValidForAutosuggest_NoInNoAt_Throws()
    {
        var options = new AutosuggestOptions { In = null, At = null };

        Assert.Throws<InvalidOperationException>(() => options.EnsureValidForAutosuggest());
    }

    [Test]
    public void EnsureValidForAutosuggest_CircleWithoutAt_DoesNotThrow()
    {
        var options = new AutosuggestOptions { In = "circle:52.5,13.4;r=10000", At = null };

        Assert.That(() => options.EnsureValidForAutosuggest(), Throws.Nothing);
    }

    [Test]
    public void EnsureValidForAutosuggest_BboxWithoutAt_DoesNotThrow()
    {
        var options = new AutosuggestOptions { In = "bbox:13.08,52.33,13.76,52.67", At = null };

        Assert.That(() => options.EnsureValidForAutosuggest(), Throws.Nothing);
    }

    [Test]
    public void InProvidesSpatialContext_DetectsCircleAndBbox()
    {
        Assert.That(new AutosuggestOptions { In = "circle:52.5,13.4;r=10000" }.InProvidesSpatialContext(), Is.True);
        Assert.That(new AutosuggestOptions { In = "bbox:13.08,52.33,13.76,52.67" }.InProvidesSpatialContext(), Is.True);
        Assert.That(new AutosuggestOptions { In = "countryCode:DEU;circle:52.5,13.4;r=10000" }.InProvidesSpatialContext(), Is.True);
        Assert.That(new AutosuggestOptions { In = " circle:52.5,13.4;r=10000 " }.InProvidesSpatialContext(), Is.True);
        Assert.That(new AutosuggestOptions { In = "countryCode:DEU" }.InProvidesSpatialContext(), Is.False);
        Assert.That(new AutosuggestOptions { In = null }.InProvidesSpatialContext(), Is.False);
    }

    [Test]
    public void InProvidesSpatialContext_IgnoresEmbeddedSubstrings()
    {
        // Raw substring matches must not count — only clauses that actually
        // start with circle:/bbox: provide a spatial context.
        Assert.That(new AutosuggestOptions { In = "notcircle:52.5,13.4;r=10000" }.InProvidesSpatialContext(), Is.False);
        Assert.That(new AutosuggestOptions { In = "foo=bbox:13.08,52.33,13.76,52.67" }.InProvidesSpatialContext(), Is.False);
        Assert.That(new AutosuggestOptions { In = "countryCode:circle:" }.InProvidesSpatialContext(), Is.False);
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
            In = "countryCode:USA",
            Show = "details,streetInfo"
        };

        var json = Helper.SerializeObject(options);

        Assert.That(json, Does.Contain("\"limit\":10"));
        Assert.That(json, Does.Contain("\"lang\":\"en\""));
        Assert.That(json, Does.Contain("\"in\":\"countryCode:USA\""));
        Assert.That(json, Does.Contain("\"show\":\"details,streetInfo\""));
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
            Show = "details",
            At = new LatLngLiteral(41.9028, 12.4964)
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<AutosuggestOptions>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Limit, Is.EqualTo(original.Limit));
        Assert.That(result.Lang, Is.EqualTo(original.Lang));
        Assert.That(result.In, Is.EqualTo(original.In));
        Assert.That(result.Show, Is.EqualTo(original.Show));
        Assert.That(result.At, Is.EqualTo(original.At));
    }
}
