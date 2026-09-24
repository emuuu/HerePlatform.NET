using System;
using System.Collections.Generic;

namespace HerePlatform.Blazor.Maps.Search;

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
    /// <c>autocomplete</c>, <c>oninput</c>, <c>onkeydown</c>, and the <c>data-here-autosuggest-input</c>
    /// marker. Splatting is mandatory — without the marker the component cannot attach its keydown
    /// listener, and Enter would submit a surrounding <c>&lt;form&gt;</c>/<c>EditForm</c> while the
    /// dropdown is open.
    /// </summary>
    public Dictionary<string, object> InputAttributes { get; init; } = new();

    /// <summary>
    /// Always <c>false</c>. Deprecated since 1.3.1: the component suppresses the browser default itself,
    /// key-selectively for <c>Enter</c> and <c>ArrowUp</c>/<c>ArrowDown</c> while the dropdown is open.
    /// A custom <see cref="HereAutosuggest.InputTemplate"/> must NOT apply
    /// <c>@onkeydown:preventDefault</c>: Blazor's modifier is a static per-event/per-element flag, so it
    /// suppresses EVERY keystroke — characters, Backspace, Tab — once the dropdown is open.
    /// Splat <see cref="InputAttributes"/> instead; it carries the marker the component needs.
    /// </summary>
    [Obsolete("No longer needed since 1.3.1: the component suppresses the browser default for Enter and " +
              "ArrowUp/ArrowDown itself. Remove @onkeydown:preventDefault from custom templates (it blocks " +
              "every keystroke) and only splat @attributes=\"context.InputAttributes\". Always false; " +
              "scheduled for removal in the next major version.", error: false)]
    public bool PreventDefaultKeyDown
    {
        get => false;
        // Accepts and discards the value so existing object initializers keep compiling.
        init { }
    }
}
