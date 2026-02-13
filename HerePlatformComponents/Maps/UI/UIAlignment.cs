using System.Runtime.Serialization;

namespace HerePlatformComponents.Maps.UI;

/// <summary>
/// Alignment position for UI controls on the map.
/// </summary>
public enum UIAlignment
{
    [EnumMember(Value = "top-left")]
    TopLeft,

    [EnumMember(Value = "top-center")]
    TopCenter,

    [EnumMember(Value = "top-right")]
    TopRight,

    [EnumMember(Value = "left-top")]
    LeftTop,

    [EnumMember(Value = "left-middle")]
    LeftMiddle,

    [EnumMember(Value = "left-bottom")]
    LeftBottom,

    [EnumMember(Value = "right-top")]
    RightTop,

    [EnumMember(Value = "right-middle")]
    RightMiddle,

    [EnumMember(Value = "right-bottom")]
    RightBottom,

    [EnumMember(Value = "bottom-left")]
    BottomLeft,

    [EnumMember(Value = "bottom-center")]
    BottomCenter,

    [EnumMember(Value = "bottom-right")]
    BottomRight
}
