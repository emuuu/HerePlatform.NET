using HerePlatform.Core.MapImage;

namespace HerePlatform.Core.Services;

/// <summary>
/// Generate static map images via the HERE Map Image API v3.
/// </summary>
[HereApi("Map Image API", "v3")]
public interface IMapImageService
{
    /// <summary>
    /// Get a static map image as a byte array.
    /// </summary>
    Task<byte[]> GetImageAsync(MapImageRequest request, CancellationToken cancellationToken = default);
}
