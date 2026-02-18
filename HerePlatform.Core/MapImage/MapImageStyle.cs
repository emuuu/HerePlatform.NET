using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using HerePlatform.Core.Serialization;

namespace HerePlatform.Core.MapImage;

/// <summary>
/// Map style for static map images.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<MapImageStyle>))]
public enum MapImageStyle
{
    [EnumMember(Value = "explore.day")]
    Default,

    [EnumMember(Value = "explore.night")]
    Night,

    [EnumMember(Value = "explore.satellite.day")]
    Satellite,

    [EnumMember(Value = "lite.day")]
    Lite,

    [EnumMember(Value = "lite.night")]
    LiteNight
}
