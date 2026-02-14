using System.Text.Json;
using System.Text.Json.Serialization;
using HerePlatformComponents.Serialization;
using OneOf;

namespace HerePlatformComponents.Tests.Serialization;

[TestFixture]
public class OneOfConverterRoundtripTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new OneOfConverterFactory() }
    };

    private record Alpha(string Name);
    private record Beta(int Value);
    private record Gamma(bool Flag);

    // --- OneOf<T0, T1> roundtrip ---

    [Test]
    public void OneOf2_Roundtrip_T0_PreservesValue()
    {
        OneOf<Alpha, Beta> original = new Alpha("hello");

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<OneOf<Alpha, Beta>>(json, Options);

        Assert.That(deserialized.IsT0, Is.True);
        Assert.That(deserialized.AsT0.Name, Is.EqualTo("hello"));
    }

    [Test]
    public void OneOf2_Roundtrip_T1_PreservesValue()
    {
        OneOf<Alpha, Beta> original = new Beta(42);

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<OneOf<Alpha, Beta>>(json, Options);

        Assert.That(deserialized.IsT1, Is.True);
        Assert.That(deserialized.AsT1.Value, Is.EqualTo(42));
    }

    // --- OneOf<T0, T1, T2> roundtrip ---

    [Test]
    public void OneOf3_Roundtrip_T0_PreservesValue()
    {
        OneOf<Alpha, Beta, Gamma> original = new Alpha("test");

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<OneOf<Alpha, Beta, Gamma>>(json, Options);

        Assert.That(deserialized.IsT0, Is.True);
        Assert.That(deserialized.AsT0.Name, Is.EqualTo("test"));
    }

    [Test]
    public void OneOf3_Roundtrip_T2_PreservesValue()
    {
        OneOf<Alpha, Beta, Gamma> original = new Gamma(true);

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<OneOf<Alpha, Beta, Gamma>>(json, Options);

        Assert.That(deserialized.IsT2, Is.True);
        Assert.That(deserialized.AsT2.Flag, Is.True);
    }

    // --- dotnetTypeName path ---

    [Test]
    public void OneOf2_Read_WithDotnetTypeName_Deserializes()
    {
        var json = $$"""{"name":"fromTypeName","dotnetTypeName":"{{typeof(Alpha).FullName}}"}""";

        var result = JsonSerializer.Deserialize<OneOf<Alpha, Beta>>(json, Options);

        Assert.That(result.IsT0, Is.True);
        Assert.That(result.AsT0.Name, Is.EqualTo("fromTypeName"));
    }

    // --- $index path ---

    [Test]
    public void OneOf2_Read_WithIndex_Deserializes()
    {
        var json = """{"$index":1,"value":99}""";

        var result = JsonSerializer.Deserialize<OneOf<Alpha, Beta>>(json, Options);

        Assert.That(result.IsT1, Is.True);
        Assert.That(result.AsT1.Value, Is.EqualTo(99));
    }

    // --- Error case ---

    [Test]
    public void OneOf2_Read_WithoutIndexOrTypeName_Throws()
    {
        var json = """{"name":"orphan"}""";

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<OneOf<Alpha, Beta>>(json, Options));
    }

    // --- Write includes dotnetTypeName ---

    [Test]
    public void OneOf2_Write_IncludesDotnetTypeName()
    {
        OneOf<Alpha, Beta> value = new Alpha("written");

        var json = JsonSerializer.Serialize(value, Options);
        using var doc = JsonDocument.Parse(json);

        Assert.That(doc.RootElement.TryGetProperty("dotnetTypeName", out var prop), Is.True);
        Assert.That(prop.GetString(), Is.EqualTo(typeof(Alpha).FullName));
    }
}
