using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Serialization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HerePlatformComponents.Maps.Services.Isoline;

/// <summary>
/// Range type for isoline calculation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<IsolineRangeType>))]
public enum IsolineRangeType
{
    [EnumMember(Value = "time")]
    Time,

    [EnumMember(Value = "distance")]
    Distance,

    [EnumMember(Value = "consumption")]
    Consumption
}
