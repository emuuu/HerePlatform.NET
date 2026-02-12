namespace HerePlatformComponents.Maps.Search;

/// <summary>
/// Predefined design variants for the <see cref="HereAutosuggest"/> component.
/// </summary>
public enum AutosuggestDesign
{
    /// <summary>
    /// Standard Bootstrap input with title + address in dropdown items.
    /// </summary>
    Default,

    /// <summary>
    /// Smaller font, reduced padding, single-line items (title only).
    /// </summary>
    Compact,

    /// <summary>
    /// Input with gray background, borderless until focused.
    /// </summary>
    Filled,

    /// <summary>
    /// Thicker border with accent color on focus.
    /// </summary>
    Outlined,

    /// <summary>
    /// Pill-shaped input with rounded dropdown corners.
    /// </summary>
    Rounded
}
