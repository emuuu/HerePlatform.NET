using System.Runtime.Serialization;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Transport mode for routing.
/// </summary>
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
