using HerePlatform.Core.Places;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Discover, browse, and look up places via the HERE Places API v1.
/// </summary>
public interface IPlacesService
{
    /// <summary>
    /// Free-text search for places near a given position.
    /// </summary>
    Task<PlacesResult> DiscoverAsync(PlacesRequest request);

    /// <summary>
    /// Browse places by category near a given position.
    /// </summary>
    Task<PlacesResult> BrowseAsync(PlacesRequest request);

    /// <summary>
    /// Look up a single place by its HERE Place ID.
    /// </summary>
    Task<PlacesResult> LookupAsync(PlacesRequest request);
}
