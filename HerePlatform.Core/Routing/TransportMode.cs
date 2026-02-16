using HerePlatform.Core.Serialization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HerePlatform.Core.Routing;

/// <summary>
/// Transport mode for routing.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<TransportMode>))]
public enum TransportMode
{
    [EnumMember(Value = "car")]
    Car,

    [EnumMember(Value = "truck")]
    Truck,

    [EnumMember(Value = "pedestrian")]
    Pedestrian,

    [EnumMember(Value = "bicycle")]
    Bicycle,

    [EnumMember(Value = "scooter")]
    Scooter
}
