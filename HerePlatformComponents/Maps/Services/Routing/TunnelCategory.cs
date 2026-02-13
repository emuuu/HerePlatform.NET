using System.Runtime.Serialization;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// ADR tunnel restriction category.
/// </summary>
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
