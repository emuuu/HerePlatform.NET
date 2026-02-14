using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HerePlatformComponents.Serialization;

internal class JsObjectRefConverter<T> : JsonConverter<T>
    where T : IJsObjectRef
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException(
            "IJsObjectRef cannot be deserialized directly. " +
            "Use JsInteropHelper.MyInvokeAsync<TRes>() which resolves objects via the GUID registry.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        using var doc = JsonSerializer.SerializeToDocument(new JsObjectRefDto(value.Guid), typeof(JsObjectRefDto), options);

        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            prop.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}
