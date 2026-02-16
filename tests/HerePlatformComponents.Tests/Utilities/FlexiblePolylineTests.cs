using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Utilities;

namespace HerePlatformComponents.Tests.Utilities;

[TestFixture]
public class FlexiblePolylineTests
{
    [Test]
    public void Decode_EmptyString_ReturnsEmptyList()
    {
        var result = FlexiblePolyline.Decode("");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Decode_NullString_ReturnsEmptyList()
    {
        var result = FlexiblePolyline.Decode(null!);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Encode_EmptyList_ProducesNonEmptyHeader()
    {
        var result = FlexiblePolyline.Encode(new List<LatLngLiteral>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.GreaterThan(0));
    }

    /// <summary>
    /// Reference test vector from https://github.com/heremaps/flexible-polyline test data.
    /// Line 8: "BA3E0E" → [(-76.0, 74.0)] with precision=0, 2D.
    /// </summary>
    [Test]
    public void Decode_ReferenceVector_2D()
    {
        var decoded = FlexiblePolyline.Decode("BA3E0E");

        Assert.That(decoded, Has.Count.EqualTo(1));
        Assert.That(decoded[0].Lat, Is.EqualTo(-76.0).Within(0.1));
        Assert.That(decoded[0].Lng, Is.EqualTo(74.0).Within(0.1));
    }

    [Test]
    public void RoundTrip_SinglePoint()
    {
        var original = new List<LatLngLiteral> { new(52.52000, 13.40500) };

        var encoded = FlexiblePolyline.Encode(original);
        var decoded = FlexiblePolyline.Decode(encoded);

        Assert.That(decoded, Has.Count.EqualTo(1));
        Assert.That(decoded[0].Lat, Is.EqualTo(52.52).Within(0.00001));
        Assert.That(decoded[0].Lng, Is.EqualTo(13.405).Within(0.00001));
    }

    [Test]
    public void RoundTrip_MultiplePoints()
    {
        var original = new List<LatLngLiteral>
        {
            new(52.52000, 13.40500),
            new(48.85660, 2.35220),
            new(51.50740, -0.12780)
        };

        var encoded = FlexiblePolyline.Encode(original);
        var decoded = FlexiblePolyline.Decode(encoded);

        Assert.That(decoded, Has.Count.EqualTo(3));
        Assert.That(decoded[0].Lat, Is.EqualTo(52.52).Within(0.00001));
        Assert.That(decoded[1].Lat, Is.EqualTo(48.8566).Within(0.00001));
        Assert.That(decoded[1].Lng, Is.EqualTo(2.3522).Within(0.00001));
        Assert.That(decoded[2].Lat, Is.EqualTo(51.5074).Within(0.00001));
        Assert.That(decoded[2].Lng, Is.EqualTo(-0.1278).Within(0.00001));
    }

    [Test]
    public void RoundTrip_NegativeCoordinates()
    {
        var original = new List<LatLngLiteral>
        {
            new(-33.8688, 151.2093),  // Sydney
            new(-22.9068, -43.1729)   // Rio de Janeiro
        };

        var encoded = FlexiblePolyline.Encode(original);
        var decoded = FlexiblePolyline.Decode(encoded);

        Assert.That(decoded, Has.Count.EqualTo(2));
        Assert.That(decoded[0].Lat, Is.EqualTo(-33.8688).Within(0.00001));
        Assert.That(decoded[0].Lng, Is.EqualTo(151.2093).Within(0.00001));
        Assert.That(decoded[1].Lat, Is.EqualTo(-22.9068).Within(0.00001));
        Assert.That(decoded[1].Lng, Is.EqualTo(-43.1729).Within(0.00001));
    }

    [Test]
    public void RoundTrip_HighPrecision()
    {
        var original = new List<LatLngLiteral>
        {
            new(52.51234, 13.41234)
        };

        var encoded = FlexiblePolyline.Encode(original, precision: 7);
        var decoded = FlexiblePolyline.Decode(encoded);

        Assert.That(decoded, Has.Count.EqualTo(1));
        Assert.That(decoded[0].Lat, Is.EqualTo(52.51234).Within(0.0000001));
        Assert.That(decoded[0].Lng, Is.EqualTo(13.41234).Within(0.0000001));
    }

    [Test]
    public void RoundTrip_BerlinToMunich()
    {
        var original = new List<LatLngLiteral>
        {
            new(52.52, 13.405),   // Berlin
            new(51.05, 12.37),    // Leipzig area
            new(50.11, 11.77),    // Bamberg area
            new(48.77, 11.43),    // Ingolstadt area
            new(48.1351, 11.582)  // Munich
        };

        var encoded = FlexiblePolyline.Encode(original);
        var decoded = FlexiblePolyline.Decode(encoded);

        Assert.That(decoded, Has.Count.EqualTo(5));
        Assert.That(decoded[0].Lat, Is.EqualTo(52.52).Within(0.00001));
        Assert.That(decoded[4].Lat, Is.EqualTo(48.1351).Within(0.00001));
        Assert.That(decoded[4].Lng, Is.EqualTo(11.582).Within(0.00001));
    }

    [Test]
    public void Encode_ProducesCompactString()
    {
        var coords = new List<LatLngLiteral>
        {
            new(52.52, 13.405),
            new(52.521, 13.406),
            new(52.522, 13.407)
        };

        var encoded = FlexiblePolyline.Encode(coords);

        Assert.That(encoded.Length, Is.LessThan(50));
    }
}
