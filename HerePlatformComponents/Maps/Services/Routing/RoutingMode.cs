using System.Runtime.Serialization;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Routing optimization mode.
/// </summary>
public enum RoutingMode
{
    [EnumMember(Value = "fast")]
    Fast,

    [EnumMember(Value = "short")]
    Short
}
