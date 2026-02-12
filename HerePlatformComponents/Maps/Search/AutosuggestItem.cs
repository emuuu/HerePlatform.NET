namespace HerePlatformComponents.Maps.Search;

/// <summary>
/// A single result item from the HERE Autosuggest API.
/// </summary>
public class AutosuggestItem
{
    /// <summary>
    /// Display label for the suggestion.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// HERE place ID (can be used for lookup/details).
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Result type: "place", "street", "houseNumber", "locality", etc.
    /// </summary>
    public string? ResultType { get; set; }

    /// <summary>
    /// Structured address details.
    /// </summary>
    public AutosuggestAddress? Address { get; set; }

    /// <summary>
    /// Geographic coordinates of the result, if available.
    /// May be null for some result types (e.g. categoryQuery).
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Highlight ranges for matching text portions.
    /// </summary>
    public AutosuggestHighlights? Highlights { get; set; }
}

/// <summary>
/// Highlight information indicating which portions of the title match the query.
/// </summary>
public class AutosuggestHighlights
{
    /// <summary>
    /// Highlight ranges for the title field.
    /// </summary>
    public AutosuggestHighlightRange[]? Title { get; set; }

    /// <summary>
    /// Highlight ranges for the address label.
    /// </summary>
    public AutosuggestHighlightRange[]? Address { get; set; }
}

/// <summary>
/// A character range within a string that should be highlighted.
/// </summary>
public class AutosuggestHighlightRange
{
    /// <summary>
    /// Start index (inclusive) of the highlighted range.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// End index (exclusive) of the highlighted range.
    /// </summary>
    public int End { get; set; }
}
