using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geofencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatform.Core.Services;

/// <summary>
/// Client-side geofence checks — point-in-polygon and point-in-circle without any HTTP call.
/// </summary>
public interface IGeofencingService
{
    /// <summary>
    /// Check whether a position falls inside any of the given geofence zones (polygon or circle).
    /// </summary>
    /// <param name="position">Position to check.</param>
    /// <param name="zones">Geofence zones (polygon or circle).</param>
    Task<GeofenceCheckResult> CheckPositionAsync(LatLngLiteral position, List<GeofenceZone> zones);
}
