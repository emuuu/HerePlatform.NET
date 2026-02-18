using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using HerePlatform.Core.Serialization;

namespace HerePlatform.Core.EvChargePoints;

/// <summary>
/// EV connector type identifiers used by the HERE EV Charge Points API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<ConnectorType>))]
public enum ConnectorType
{
    [EnumMember(Value = "2")]
    Type2,

    [EnumMember(Value = "3")]
    Chademo,

    [EnumMember(Value = "22")]
    CcsCombo1,

    [EnumMember(Value = "33")]
    CcsCombo2,

    [EnumMember(Value = "7")]
    TeslaSupercharger
}
