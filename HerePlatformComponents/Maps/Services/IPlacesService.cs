using HerePlatformComponents.Maps.Services.Places;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Service for HERE Places API (discover, browse, lookup).
/// </summary>
public interface IPlacesService
{
    /// <summary>
    /// Free-text search for places (H.service.SearchService.discover).
    /// </summary>
    Task<PlacesResult> DiscoverAsync(PlacesRequest request);

    /// <summary>
    /// Browse places by category (H.service.SearchService.browse).
    /// </summary>
    Task<PlacesResult> BrowseAsync(PlacesRequest request);

    /// <summary>
    /// Look up a single place by HERE Place ID (H.service.SearchService.lookup).
    /// </summary>
    Task<PlacesResult> LookupAsync(PlacesRequest request);
}
