using System.Text.Json;
using HerePlatformComponents.Maps.Services.Isoline;
using HerePlatformComponents.Maps.Services.MatrixRouting;
using HerePlatformComponents.Maps.Services.Routing;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Serialization;

/// <summary>
/// Tests that request objects serialize correctly for JS interop.
/// Blazor's IJSRuntime uses System.Text.Json with default options (camelCase),
/// NOT the custom Helper.Options. These tests catch enum-as-integer bugs
/// and incorrect API parameter values before they reach the HERE API.
/// </summary>
[TestFixture]
public class JsInteropSerializationTests
{
    /// <summary>
    /// Mimics Blazor's IJSRuntime serialization: camelCase properties, default handling.
    /// </summary>
    private static readonly JsonSerializerOptions BlazorJsOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // --- TransportMode ---

    [TestCase(TransportMode.Car, "car")]
    [TestCase(TransportMode.Truck, "truck")]
    [TestCase(TransportMode.Pedestrian, "pedestrian")]
    [TestCase(TransportMode.Bicycle, "bicycle")]
    [TestCase(TransportMode.Scooter, "scooter")]
    public void TransportMode_SerializesAsString(TransportMode mode, string expected)
    {
        var json = JsonSerializer.Serialize(mode, BlazorJsOptions);
        Assert.That(json, Is.EqualTo($"\"{expected}\""));
    }

    [TestCase("\"car\"", TransportMode.Car)]
    [TestCase("\"truck\"", TransportMode.Truck)]
    [TestCase("\"pedestrian\"", TransportMode.Pedestrian)]
    [TestCase("\"bicycle\"", TransportMode.Bicycle)]
    public void TransportMode_DeserializesFromString(string json, TransportMode expected)
    {
        var result = JsonSerializer.Deserialize<TransportMode>(json, BlazorJsOptions);
        Assert.That(result, Is.EqualTo(expected));
    }

    // --- RoutingMode ---

    [TestCase(RoutingMode.Fast, "fast")]
    [TestCase(RoutingMode.Short, "short")]
    public void RoutingMode_SerializesAsString(RoutingMode mode, string expected)
    {
        var json = JsonSerializer.Serialize(mode, BlazorJsOptions);
        Assert.That(json, Is.EqualTo($"\"{expected}\""));
    }

    // --- IsolineRangeType ---

    [TestCase(IsolineRangeType.Time, "time")]
    [TestCase(IsolineRangeType.Distance, "distance")]
    [TestCase(IsolineRangeType.Consumption, "consumption")]
    public void IsolineRangeType_SerializesAsString(IsolineRangeType type, string expected)
    {
        var json = JsonSerializer.Serialize(type, BlazorJsOptions);
        Assert.That(json, Is.EqualTo($"\"{expected}\""));
    }

    // --- TunnelCategory ---

    [TestCase(TunnelCategory.B, "B")]
    [TestCase(TunnelCategory.C, "C")]
    [TestCase(TunnelCategory.D, "D")]
    [TestCase(TunnelCategory.E, "E")]
    public void TunnelCategory_SerializesAsString(TunnelCategory cat, string expected)
    {
        var json = JsonSerializer.Serialize(cat, BlazorJsOptions);
        Assert.That(json, Is.EqualTo($"\"{expected}\""));
    }

    // --- RoutingRequest (full object) ---

    [Test]
    public void RoutingRequest_TransportMode_SerializesAsString_NotInteger()
    {
        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.52, 13.405),
            Destination = new LatLngLiteral(48.1351, 11.582),
            TransportMode = TransportMode.Truck
        };

        var json = JsonSerializer.Serialize(request, BlazorJsOptions);

        Assert.That(json, Does.Contain("\"transportMode\":\"truck\""));
        Assert.That(json, Does.Not.Contain("\"transportMode\":1"));
    }

    [Test]
    public void RoutingRequest_RoutingMode_SerializesAsString_NotInteger()
    {
        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.52, 13.405),
            Destination = new LatLngLiteral(48.1351, 11.582),
            RoutingMode = RoutingMode.Short
        };

        var json = JsonSerializer.Serialize(request, BlazorJsOptions);

        Assert.That(json, Does.Contain("\"routingMode\":\"short\""));
        Assert.That(json, Does.Not.Contain("\"routingMode\":1"));
    }

    [Test]
    public void RoutingRequest_AllTransportModes_NeverSerializeAsIntegers()
    {
        foreach (var mode in Enum.GetValues<TransportMode>())
        {
            var request = new RoutingRequest
            {
                Origin = new LatLngLiteral(0, 0),
                Destination = new LatLngLiteral(0, 0),
                TransportMode = mode
            };

            var json = JsonSerializer.Serialize(request, BlazorJsOptions);

            Assert.That(json, Does.Not.Match(@"""transportMode"":\d"),
                $"TransportMode.{mode} serialized as integer");
        }
    }

    // --- MatrixRoutingRequest ---

    [Test]
    public void MatrixRoutingRequest_Enums_SerializeAsStrings()
    {
        var request = new MatrixRoutingRequest
        {
            Origins = [new LatLngLiteral(52.52, 13.405)],
            Destinations = [new LatLngLiteral(48.1351, 11.582)],
            TransportMode = TransportMode.Bicycle,
            RoutingMode = RoutingMode.Short
        };

        var json = JsonSerializer.Serialize(request, BlazorJsOptions);

        Assert.That(json, Does.Contain("\"transportMode\":\"bicycle\""));
        Assert.That(json, Does.Contain("\"routingMode\":\"short\""));
    }

    // --- EV Options value ranges ---

    [Test]
    public void EvOptions_MaxCharge_AcceptsKwhValues()
    {
        var ev = new EvOptions
        {
            InitialCharge = 48,
            MaxCharge = 64,
            MinChargeAtDestination = 6,
            AuxiliaryConsumption = 1.6
        };

        // HERE API constraint: maxCharge <= 1200 (kWh)
        Assert.That(ev.MaxCharge, Is.LessThanOrEqualTo(1200),
            "MaxCharge should be in kWh, not Wh");
        Assert.That(ev.InitialCharge, Is.LessThanOrEqualTo(ev.MaxCharge),
            "InitialCharge should not exceed MaxCharge");
    }

    [Test]
    public void EvOptions_Serializes_WithDoubleValues()
    {
        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.52, 13.405),
            Destination = new LatLngLiteral(48.1351, 11.582),
            Ev = new EvOptions
            {
                InitialCharge = 48.5,
                MaxCharge = 64,
                AuxiliaryConsumption = 1.6
            }
        };

        var json = JsonSerializer.Serialize(request, BlazorJsOptions);

        Assert.That(json, Does.Contain("\"initialCharge\":48.5"));
        Assert.That(json, Does.Contain("\"maxCharge\":64"));
        Assert.That(json, Does.Contain("\"auxiliaryConsumption\":1.6"));
    }

    // --- TruckOptions with enum fields ---

    [Test]
    public void TruckOptions_TunnelCategory_SerializesAsString()
    {
        var request = new RoutingRequest
        {
            Origin = new LatLngLiteral(52.52, 13.405),
            Destination = new LatLngLiteral(48.1351, 11.582),
            TransportMode = TransportMode.Truck,
            Truck = new TruckOptions
            {
                Height = 4.0,
                TunnelCategory = TunnelCategory.C
            }
        };

        var json = JsonSerializer.Serialize(request, BlazorJsOptions);

        Assert.That(json, Does.Contain("\"tunnelCategory\":\"C\""));
        Assert.That(json, Does.Not.Contain("\"tunnelCategory\":1"));
    }
}
