using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Search;

/// <summary>
/// Context object passed to <see cref="HereAutosuggest.SuggestionListTemplate"/>,
/// providing everything needed to render a fully custom suggestion dropdown.
/// </summary>
public class AutosuggestListContext
{
    /// <summary>
    /// The current list of suggestion items.
    /// </summary>
    public IReadOnlyList<AutosuggestItem> Items { get; init; } = Array.Empty<AutosuggestItem>();

    /// <summary>
    /// Index of the currently highlighted item (-1 if none).
    /// </summary>
    public int ActiveIndex { get; init; } = -1;

    /// <summary>
    /// Call this to select an item (updates the input value and fires OnItemSelected).
    /// Use with <c>@onmousedown</c> + <c>@onmousedown:preventDefault</c> on your custom items.
    /// </summary>
    public Func<AutosuggestItem, Task> SelectItem { get; init; } = _ => Task.CompletedTask;
}
