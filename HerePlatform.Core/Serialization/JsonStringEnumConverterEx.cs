using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HerePlatform.Core.Serialization;

public class JsonStringEnumConverterEx<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, string> _enumToString = new Dictionary<TEnum, string>();
    private readonly Dictionary<string, TEnum> _stringToEnum = new Dictionary<string, TEnum>();

    public JsonStringEnumConverterEx()
    {
        var type = typeof(TEnum);
        var values = Enum.GetValues(typeof(TEnum));

        foreach (var value in values)
        {
            var enumMember = type.GetMember(value.ToString()!)[0];
            var attr = enumMember
                .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            _stringToEnum[value.ToString()!] = (TEnum)value;

            if (attr?.Value != null)
            {
                _enumToString[(TEnum)value] = attr.Value;
                _stringToEnum[attr.Value] = (TEnum)value;
            }
            else
            {
                _enumToString[(TEnum)value] = value.ToString()!;
            }
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (_stringToEnum.TryGetValue(stringValue ?? "", out var result))
            return result;

        System.Diagnostics.Debug.WriteLine(
            $"[BlazorHerePlatform] Unknown enum value '{stringValue}' for {typeof(TEnum).Name}, falling back to default");
        return default;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(_enumToString[value]);
    }
}
