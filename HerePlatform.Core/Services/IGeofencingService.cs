using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geofencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Service for geofence checks (client-side point-in-polygon/circle).
/// </summary>
public interface IGeofencingService
{
    /// <summary>
    /// Check if a position is inside any of the given geofence zones.
    /// </summary>
    Task<GeofenceCheckResult> CheckPositionAsync(LatLngLiteral position, List<GeofenceZone> zones);
}
