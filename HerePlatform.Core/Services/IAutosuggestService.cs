using HerePlatform.Core.Search;

namespace HerePlatform.Core.Services;

/// <summary>
/// Autosuggest and autocomplete via the HERE Geocoding &amp; Search API v7.
/// </summary>
[HereApi("Geocoding & Search API", "v7")]
public interface IAutosuggestService
{
    /// <summary>
    /// Get location suggestions as the user types.
    /// </summary>
    /// <param name="query">Partial or full search text.</param>
    /// <param name="options">Optional parameters like language, limit, and geographic filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<AutosuggestResult> SuggestAsync(string query, AutosuggestOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get autocomplete suggestions for addresses and places (no position returned).
    /// </summary>
    /// <param name="query">Partial or full search text.</param>
    /// <param name="options">Optional parameters like language, limit, and geographic filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<AutocompleteResult> AutocompleteAsync(string query, AutosuggestOptions? options = null, CancellationToken cancellationToken = default);
}
