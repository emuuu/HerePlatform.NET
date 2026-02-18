using HerePlatform.Core.Places;

namespace HerePlatform.Core.Services;

/// <summary>
/// Discover, browse, and look up places via the HERE Geocoding &amp; Search API v7.
/// </summary>
[HereApi("Geocoding & Search API", "v7")]
public interface IPlacesService
{
    /// <summary>
    /// Free-text search for places near a given position.
    /// </summary>
    Task<PlacesResult> DiscoverAsync(PlacesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Browse places by category near a given position.
    /// </summary>
    Task<PlacesResult> BrowseAsync(PlacesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Look up a single place by its HERE Place ID.
    /// </summary>
    Task<PlacesResult> LookupAsync(PlacesRequest request, CancellationToken cancellationToken = default);
}
