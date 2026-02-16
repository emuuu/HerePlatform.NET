using System;
using System.Runtime.Serialization;

namespace HerePlatform.Core.Routing;

/// <summary>
/// Features to avoid in route calculation.
/// </summary>
[Flags]
public enum RoutingAvoidFeature
{
    None = 0,

    [EnumMember(Value = "tollRoad")]
    Tolls = 1 << 0,

    [EnumMember(Value = "controlledAccessHighway")]
    Highways = 1 << 1,

    [EnumMember(Value = "ferry")]
    Ferries = 1 << 2,

    [EnumMember(Value = "tunnel")]
    Tunnels = 1 << 3
}
