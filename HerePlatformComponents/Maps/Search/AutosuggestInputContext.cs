using System.Collections.Generic;

namespace HerePlatformComponents.Maps.Search;

/// <summary>
/// Context object passed to the <see cref="HereAutosuggest.InputTemplate"/> render fragment,
/// providing all necessary bindings for a custom input element.
/// </summary>
public class AutosuggestInputContext
{
    /// <summary>
    /// Current text value of the input.
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// Placeholder text for the input.
    /// </summary>
    public string? Placeholder { get; init; }

    /// <summary>
    /// Whether the input is disabled.
    /// </summary>
    public bool Disabled { get; init; }

    /// <summary>
    /// Dictionary of HTML attributes and event handlers that must be applied to the custom input element
    /// via <c>@attributes</c> splatting. Contains <c>value</c>, <c>placeholder</c>, <c>disabled</c>,
    /// <c>autocomplete</c>, <c>oninput</c>, <c>onkeydown</c>, and <c>onfocusout</c>.
    /// </summary>
    public Dictionary<string, object> InputAttributes { get; init; } = new();
}
