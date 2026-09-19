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
    /// <c>autocomplete</c>, <c>oninput</c>, <c>onkeydown</c>, and <c>onfocusout</c>.
    /// </summary>
    public Dictionary<string, object> InputAttributes { get; init; } = new();

    /// <summary>
    /// Whether the custom input's <c>onkeydown</c> handler must suppress the browser default action
    /// (<c>true</c> whenever the suggestion dropdown is open, since Enter always consumes a suggestion —
    /// the active one, or the first when none is active). <c>@attributes</c> splatting cannot express the
    /// <c>@onkeydown:preventDefault</c> event modifier the default template applies, so a custom
    /// <see cref="HereAutosuggest.InputTemplate"/> must apply it explicitly:
    /// <c>@onkeydown:preventDefault="@context.PreventDefaultKeyDown"</c>. Without it, pressing Enter while the
    /// dropdown is open both selects a suggestion AND submits a surrounding <c>&lt;form&gt;</c>/<c>EditForm</c>.
    /// </summary>
    public bool PreventDefaultKeyDown { get; init; }
}
