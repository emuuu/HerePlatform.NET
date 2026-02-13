using System;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Flags enum for H.mapevents.Behavior.Feature.
/// Each flag corresponds to an interaction feature that can be individually enabled/disabled.
/// </summary>
[Flags]
public enum BehaviorFeature
{
    None = 0,
    Panning = 1 << 0,
    WheelZoom = 1 << 1,
    PinchZoom = 1 << 2,
    DblTapZoom = 1 << 3,
    Tilt = 1 << 4,
    Heading = 1 << 5,
    FractionalZoom = 1 << 6,

    All = Panning | WheelZoom | PinchZoom | DblTapZoom | Tilt | Heading | FractionalZoom
}
