using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geofencing;
using HerePlatform.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services;

/// <summary>
/// Client-side geofencing implementation (point-in-polygon/circle checks).
/// </summary>
public class GeofencingService : IGeofencingService
{
    public Task<GeofenceCheckResult> CheckPositionAsync(LatLngLiteral position, List<GeofenceZone> zones)
    {
        var matchedIds = new List<string>();

        foreach (var zone in zones)
        {
            bool inside = zone.Type == "circle"
                ? IsInsideCircle(position, zone)
                : IsInsidePolygon(position, zone);

            if (inside && zone.Id != null)
            {
                matchedIds.Add(zone.Id);
            }
        }

        return Task.FromResult(new GeofenceCheckResult
        {
            IsInside = matchedIds.Count > 0,
            MatchedZoneIds = matchedIds
        });
    }

    private static bool IsInsideCircle(LatLngLiteral position, GeofenceZone zone)
    {
        if (!zone.Center.HasValue) return false;

        var distance = HaversineDistance(
            position.Lat, position.Lng,
            zone.Center.Value.Lat, zone.Center.Value.Lng);

        return distance <= zone.Radius;
    }

    private static bool IsInsidePolygon(LatLngLiteral position, GeofenceZone zone)
    {
        if (zone.Vertices == null || zone.Vertices.Count < 3) return false;

        // Ray casting algorithm
        bool inside = false;
        int n = zone.Vertices.Count;

        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            var vi = zone.Vertices[i];
            var vj = zone.Vertices[j];

            if ((vi.Lng > position.Lng) != (vj.Lng > position.Lng) &&
                position.Lat < (vj.Lat - vi.Lat) * (position.Lng - vi.Lng) / (vj.Lng - vi.Lng) + vi.Lat)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    private static double HaversineDistance(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371000; // Earth radius in meters
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
