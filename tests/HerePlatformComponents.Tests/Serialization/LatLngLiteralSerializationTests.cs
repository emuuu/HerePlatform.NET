using System.Text.Json;
using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Serialization;

[TestFixture]
public class LatLngLiteralSerializationTests
{
    [Test]
    public void Serialize_ProducesCorrectJson()
    {
        var coord = new LatLngLiteral(52.52, 13.405);

        var json = JsonSerializer.Serialize(coord);

        Assert.That(json, Does.Contain("\"lat\":52.52"));
        Assert.That(json, Does.Contain("\"lng\":13.405"));
    }

    [Test]
    public void Deserialize_ParsesCorrectly()
    {
        var json = """{"lat":52.52,"lng":13.405}""";

        var coord = JsonSerializer.Deserialize<LatLngLiteral>(json);

        Assert.That(coord.Lat, Is.EqualTo(52.52));
        Assert.That(coord.Lng, Is.EqualTo(13.405));
    }

    [Test]
    public void RoundTrip_PreservesValues()
    {
        var original = new LatLngLiteral(48.8566, 2.3522);

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<LatLngLiteral>(json);

        Assert.That(deserialized, Is.EqualTo(original));
    }

    [Test]
    public void Deserialize_ZeroCoordinates()
    {
        var json = """{"lat":0,"lng":0}""";

        var coord = JsonSerializer.Deserialize<LatLngLiteral>(json);

        Assert.That(coord.Lat, Is.EqualTo(0));
        Assert.That(coord.Lng, Is.EqualTo(0));
    }

    [Test]
    public void Deserialize_NegativeCoordinates()
    {
        var json = """{"lat":-33.8688,"lng":-151.2093}""";

        var coord = JsonSerializer.Deserialize<LatLngLiteral>(json);

        Assert.That(coord.Lat, Is.EqualTo(-33.8688).Within(0.0001));
        Assert.That(coord.Lng, Is.EqualTo(-151.2093).Within(0.0001));
    }

    [Test]
    public void Deserialize_BoundaryValues()
    {
        var json = """{"lat":90,"lng":180}""";

        var coord = JsonSerializer.Deserialize<LatLngLiteral>(json);

        Assert.That(coord.Lat, Is.EqualTo(90));
        Assert.That(coord.Lng, Is.EqualTo(180));
    }

    [Test]
    public void Deserialize_ExtraProperties_AreIgnored()
    {
        var json = """{"lat":52.52,"lng":13.405,"alt":100}""";

        var coord = JsonSerializer.Deserialize<LatLngLiteral>(json);

        Assert.That(coord.Lat, Is.EqualTo(52.52));
        Assert.That(coord.Lng, Is.EqualTo(13.405));
    }

    [Test]
    public void Deserialize_MissingLat_ThrowsJsonException()
    {
        var json = """{"lng":13.405}""";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<LatLngLiteral>(json));
    }

    [Test]
    public void Deserialize_MissingLng_ThrowsJsonException()
    {
        var json = """{"lat":52.52}""";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<LatLngLiteral>(json));
    }

    [Test]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        var json = "not json";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<LatLngLiteral>(json));
    }

    [Test]
    public void Serialize_Array_ProducesCorrectJson()
    {
        var coords = new[]
        {
            new LatLngLiteral(52.52, 13.405),
            new LatLngLiteral(48.8566, 2.3522)
        };

        var json = JsonSerializer.Serialize(coords);
        var deserialized = JsonSerializer.Deserialize<LatLngLiteral[]>(json);

        Assert.That(deserialized, Has.Length.EqualTo(2));
        Assert.That(deserialized![0], Is.EqualTo(coords[0]));
        Assert.That(deserialized[1], Is.EqualTo(coords[1]));
    }
}
