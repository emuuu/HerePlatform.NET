using System.Runtime.Serialization;

namespace HerePlatformComponents.Maps.Services.Isoline;

/// <summary>
/// Range type for isoline calculation.
/// </summary>
public enum IsolineRangeType
{
    [EnumMember(Value = "time")]
    Time,

    [EnumMember(Value = "distance")]
    Distance,

    [EnumMember(Value = "consumption")]
    Consumption
}
