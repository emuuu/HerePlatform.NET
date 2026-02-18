using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using HerePlatform.Core.Serialization;

namespace HerePlatform.Core.MapImage;

/// <summary>
/// Output format for static map images.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<MapImageFormat>))]
public enum MapImageFormat
{
    [EnumMember(Value = "png")]
    Png,

    [EnumMember(Value = "jpeg")]
    Jpeg
}
