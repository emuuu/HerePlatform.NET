using HerePlatform.Core.Serialization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HerePlatform.Core.Routing;

/// <summary>
/// Routing optimization mode.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<RoutingMode>))]
public enum RoutingMode
{
    [EnumMember(Value = "fast")]
    Fast,

    [EnumMember(Value = "short")]
    Short
}
