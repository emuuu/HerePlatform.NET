using HerePlatformComponents.Serialization;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HerePlatformComponents;

internal static partial class Helper
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new OneOfConverterFactory(),
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        }
    };

    public static object? DeSerializeObject(JsonElement json, Type type)
    {
        var obj = json.Deserialize(type, Options);
        return obj;
    }

    public static object? DeSerializeObject(string? json, Type? type)
    {
        if (json == null || type == null)
        {
            return default;
        }

        var obj = JsonSerializer.Deserialize(json, type, Options);
        return obj;
    }

    public static TObject? DeSerializeObject<TObject>(string? json)
    {
        if (json == null)
        {
            return default;
        }

        var value = JsonSerializer.Deserialize<TObject>(json, Options);
        return value;
    }

    public static string SerializeObject(object obj)
    {
        var value = JsonSerializer.Serialize(obj, Options);
        return value;
    }

    internal static T? ToNullableEnum<T>(string? str)
        where T : struct
    {
        var enumType = typeof(T);

        if (int.TryParse(str, out var enumIntValue))
        {
            return (T)Enum.Parse(enumType, enumIntValue.ToString());
        }

        if (str == "null")
        {
            return null;
        }

        foreach (var name in Enum.GetNames(enumType))
        {
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name)!.GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            if (enumMemberAttribute.Value == str)
            {
                return (T)Enum.Parse(enumType, name);
            }
        }

        return default;
    }

    internal static T? ToEnum<T>(string str)
    {
        var enumType = typeof(T);
        foreach (var name in Enum.GetNames(enumType))
        {
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name)!.GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            if (enumMemberAttribute.Value == str)
            {
                return (T)Enum.Parse(enumType, name);
            }

            if (string.Equals(name, str, StringComparison.InvariantCultureIgnoreCase))
            {
                return (T)Enum.Parse(enumType, name);
            }
        }

        return default;
    }

    internal static string? GetEnumMemberValue<T>(T enumItem) where T : Enum
        => GetEnumValue(enumItem);

    private static string? GetEnumValue(object? enumItem)
    {
        if (enumItem == null)
        {
            return null;
        }

        if (enumItem is not Enum enumItem2)
        {
            return enumItem.ToString();
        }

        var memberInfo = enumItem2.GetType().GetMember(enumItem2.ToString()!);
        if (memberInfo.Length == 0)
        {
            return null;
        }

        foreach (var attr in memberInfo[0].GetCustomAttributes(false))
        {
            if (attr is EnumMemberAttribute val)
            {
                return val.Value;
            }
        }

        return null;
    }

    private static Type? ResolveOneOfType(string typeName, params Type[] allowedTypes)
    {
        foreach (var t in allowedTypes)
        {
            if (t.FullName == typeName)
                return t;
        }
        return null;
    }
}
