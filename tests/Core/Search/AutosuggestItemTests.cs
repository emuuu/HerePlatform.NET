using HerePlatformComponents;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Search;

namespace HerePlatformComponents.Tests.Search;

[TestFixture]
public class AutosuggestItemTests
{
    [Test]
    public void Serialize_FullItem_ProducesCamelCaseJson()
    {
        var item = new AutosuggestItem
        {
            Title = "Brandenburger Tor",
            Id = "here:pds:place:276u33db-1234",
            ResultType = "place",
            Address = new AutosuggestAddress
            {
                Label = "Pariser Platz, 10117 Berlin, Deutschland",
                CountryCode = "DEU",
                City = "Berlin",
                Street = "Pariser Platz",
                PostalCode = "10117"
            },
            Position = new LatLngLiteral(52.5163, 13.3777)
        };

        var json = Helper.SerializeObject(item);

        Assert.That(json, Does.Contain("\"title\":\"Brandenburger Tor\""));
        Assert.That(json, Does.Contain("\"id\":\"here:pds:place:276u33db-1234\""));
        Assert.That(json, Does.Contain("\"resultType\":\"place\""));
        Assert.That(json, Does.Contain("\"label\":\"Pariser Platz, 10117 Berlin, Deutschland\""));
        Assert.That(json, Does.Contain("\"countryCode\":\"DEU\""));
        Assert.That(json, Does.Contain("\"city\":\"Berlin\""));
        Assert.That(json, Does.Contain("\"lat\":52.5163"));
        Assert.That(json, Does.Contain("\"lng\":13.3777"));
    }

    [Test]
    public void Serialize_NullOptionalFields_AreOmitted()
    {
        var item = new AutosuggestItem
        {
            Title = "Test"
        };

        var json = Helper.SerializeObject(item);

        Assert.That(json, Does.Contain("\"title\":\"Test\""));
        Assert.That(json, Does.Not.Contain("\"id\""));
        Assert.That(json, Does.Not.Contain("\"address\""));
        Assert.That(json, Does.Not.Contain("\"position\""));
        Assert.That(json, Does.Not.Contain("\"highlights\""));
    }

    [Test]
    public void Deserialize_FromJsJson_AllFields()
    {
        var json = """
        {
            "title":"Alexanderplatz",
            "id":"here:pds:place:276u33db-5678",
            "resultType":"street",
            "address":{
                "label":"Alexanderplatz, 10178 Berlin, Deutschland",
                "countryCode":"DEU",
                "countryName":"Deutschland",
                "state":"Berlin",
                "city":"Berlin",
                "district":"Mitte",
                "street":"Alexanderplatz",
                "postalCode":"10178",
                "houseNumber":null
            },
            "position":{"lat":52.5219,"lng":13.4132},
            "highlights":{
                "title":[{"start":0,"end":5}],
                "address":[{"start":0,"end":5}]
            }
        }
        """;

        var item = Helper.DeSerializeObject<AutosuggestItem>(json);

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Title, Is.EqualTo("Alexanderplatz"));
        Assert.That(item.Id, Is.EqualTo("here:pds:place:276u33db-5678"));
        Assert.That(item.ResultType, Is.EqualTo("street"));
        Assert.That(item.Address, Is.Not.Null);
        Assert.That(item.Address!.Label, Is.EqualTo("Alexanderplatz, 10178 Berlin, Deutschland"));
        Assert.That(item.Address.CountryCode, Is.EqualTo("DEU"));
        Assert.That(item.Address.City, Is.EqualTo("Berlin"));
        Assert.That(item.Address.District, Is.EqualTo("Mitte"));
        Assert.That(item.Address.PostalCode, Is.EqualTo("10178"));
        Assert.That(item.Position, Is.Not.Null);
        Assert.That(item.Position!.Value.Lat, Is.EqualTo(52.5219));
        Assert.That(item.Position!.Value.Lng, Is.EqualTo(13.4132));
        Assert.That(item.Highlights, Is.Not.Null);
        Assert.That(item.Highlights!.Title, Has.Length.EqualTo(1));
        Assert.That(item.Highlights.Title![0].Start, Is.EqualTo(0));
        Assert.That(item.Highlights.Title[0].End, Is.EqualTo(5));
    }

    [Test]
    public void Deserialize_MinimalItem_NullPosition()
    {
        var json = """{"title":"Some query","id":null,"resultType":"categoryQuery","address":null,"position":null}""";

        var item = Helper.DeSerializeObject<AutosuggestItem>(json);

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Title, Is.EqualTo("Some query"));
        Assert.That(item.ResultType, Is.EqualTo("categoryQuery"));
        Assert.That(item.Position, Is.Null);
        Assert.That(item.Address, Is.Null);
    }

    [Test]
    public void RoundTrip_PreservesValues()
    {
        var original = new AutosuggestItem
        {
            Title = "Potsdamer Platz",
            Id = "here:pds:place:276u33db-9999",
            ResultType = "place",
            Address = new AutosuggestAddress
            {
                Label = "Potsdamer Platz, 10785 Berlin, Deutschland",
                CountryCode = "DEU",
                City = "Berlin",
                Street = "Potsdamer Platz",
                PostalCode = "10785"
            },
            Position = new LatLngLiteral(52.5096, 13.3761)
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<AutosuggestItem>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo(original.Title));
        Assert.That(result.Id, Is.EqualTo(original.Id));
        Assert.That(result.ResultType, Is.EqualTo(original.ResultType));
        Assert.That(result.Address!.Label, Is.EqualTo(original.Address!.Label));
        Assert.That(result.Address.City, Is.EqualTo(original.Address.City));
        Assert.That(result.Position, Is.EqualTo(original.Position));
    }
}

[TestFixture]
public class AutosuggestAddressTests
{
    [Test]
    public void Serialize_FullAddress_ProducesCamelCaseJson()
    {
        var address = new AutosuggestAddress
        {
            Label = "Friedrichstr. 123, 10117 Berlin",
            CountryCode = "DEU",
            CountryName = "Deutschland",
            State = "Berlin",
            City = "Berlin",
            District = "Mitte",
            Street = "Friedrichstr.",
            PostalCode = "10117",
            HouseNumber = "123"
        };

        var json = Helper.SerializeObject(address);

        Assert.That(json, Does.Contain("\"label\":\"Friedrichstr. 123, 10117 Berlin\""));
        Assert.That(json, Does.Contain("\"countryCode\":\"DEU\""));
        Assert.That(json, Does.Contain("\"houseNumber\":\"123\""));
        Assert.That(json, Does.Contain("\"postalCode\":\"10117\""));
    }

    [Test]
    public void Serialize_NullFields_AreOmitted()
    {
        var address = new AutosuggestAddress
        {
            Label = "Berlin",
            City = "Berlin"
        };

        var json = Helper.SerializeObject(address);

        Assert.That(json, Does.Contain("\"label\":\"Berlin\""));
        Assert.That(json, Does.Contain("\"city\":\"Berlin\""));
        Assert.That(json, Does.Not.Contain("\"countryCode\""));
        Assert.That(json, Does.Not.Contain("\"street\""));
        Assert.That(json, Does.Not.Contain("\"houseNumber\""));
    }

    [Test]
    public void Deserialize_FromJsJson()
    {
        var json = """{"label":"Unter den Linden 1, 10117 Berlin","countryCode":"DEU","city":"Berlin","street":"Unter den Linden","houseNumber":"1","postalCode":"10117"}""";

        var address = Helper.DeSerializeObject<AutosuggestAddress>(json);

        Assert.That(address, Is.Not.Null);
        Assert.That(address!.Label, Is.EqualTo("Unter den Linden 1, 10117 Berlin"));
        Assert.That(address.Street, Is.EqualTo("Unter den Linden"));
        Assert.That(address.HouseNumber, Is.EqualTo("1"));
    }

    [Test]
    public void RoundTrip_PreservesValues()
    {
        var original = new AutosuggestAddress
        {
            Label = "Test Str. 42, 12345 Stadt",
            CountryCode = "DEU",
            CountryName = "Deutschland",
            State = "Bayern",
            City = "Muenchen",
            District = "Altstadt",
            Street = "Test Str.",
            PostalCode = "12345",
            HouseNumber = "42"
        };

        var json = Helper.SerializeObject(original);
        var result = Helper.DeSerializeObject<AutosuggestAddress>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Label, Is.EqualTo(original.Label));
        Assert.That(result.CountryCode, Is.EqualTo(original.CountryCode));
        Assert.That(result.CountryName, Is.EqualTo(original.CountryName));
        Assert.That(result.State, Is.EqualTo(original.State));
        Assert.That(result.City, Is.EqualTo(original.City));
        Assert.That(result.District, Is.EqualTo(original.District));
        Assert.That(result.Street, Is.EqualTo(original.Street));
        Assert.That(result.PostalCode, Is.EqualTo(original.PostalCode));
        Assert.That(result.HouseNumber, Is.EqualTo(original.HouseNumber));
    }
}
