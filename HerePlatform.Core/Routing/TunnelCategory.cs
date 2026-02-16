using HerePlatform.Core.Serialization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HerePlatform.Core.Routing;

/// <summary>
/// ADR tunnel restriction category.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<TunnelCategory>))]
public enum TunnelCategory
{
    [EnumMember(Value = "B")]
    B,

    [EnumMember(Value = "C")]
    C,

    [EnumMember(Value = "D")]
    D,

    [EnumMember(Value = "E")]
    E
}
