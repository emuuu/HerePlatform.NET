using System;
using System.Runtime.Serialization;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Hazardous goods categories (flags enum for multi-select).
/// </summary>
[Flags]
public enum HazardousGoods
{
    None = 0,

    [EnumMember(Value = "explosive")]
    Explosive = 1 << 0,

    [EnumMember(Value = "gas")]
    Gas = 1 << 1,

    [EnumMember(Value = "flammable")]
    Flammable = 1 << 2,

    [EnumMember(Value = "combustible")]
    Combustible = 1 << 3,

    [EnumMember(Value = "organic")]
    Organic = 1 << 4,

    [EnumMember(Value = "poison")]
    Poison = 1 << 5,

    [EnumMember(Value = "radioActive")]
    RadioActive = 1 << 6,

    [EnumMember(Value = "corrosive")]
    Corrosive = 1 << 7,

    [EnumMember(Value = "poisonousInhalation")]
    PoisonousInhalation = 1 << 8,

    [EnumMember(Value = "harmfulToWater")]
    HarmfulToWater = 1 << 9,

    [EnumMember(Value = "other")]
    Other = 1 << 10
}
