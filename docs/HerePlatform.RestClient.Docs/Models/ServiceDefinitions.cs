namespace HerePlatform.RestClient.Docs.Models;

public static class ServiceDefinitions
{
    public static readonly IReadOnlyList<ServiceDefinition> All =
    [
        new("Geocoding", "services/geocoding", "Forward and reverse geocoding \u2014 convert addresses to coordinates and back.", "IGeocodingService"),
        new("Routing", "services/routing", "Calculate routes with support for car, truck, pedestrian, bicycle, and EV options.", "IRoutingService"),
        new("Places", "services/places", "Discover, browse, and look up places by text search, category, or place ID.", "IPlacesService"),
        new("Isoline", "services/isoline", "Calculate reachable areas (isochrones/isodistances) from a center point.", "IIsolineService"),
        new("Matrix Routing", "services/matrix-routing", "Compute travel time and distance matrices between multiple origins and destinations.", "IMatrixRoutingService"),
        new("Traffic", "services/traffic", "Get real-time traffic incidents and flow data within a bounding box.", "ITrafficService"),
        new("Public Transit", "services/public-transit", "Search for transit stations and get live departure information.", "IPublicTransitService"),
        new("Waypoint Sequence", "services/waypoint-sequence", "Optimize the order of waypoints for minimum travel time or distance.", "IWaypointSequenceService"),
        new("Geofencing", "services/geofencing", "Client-side point-in-polygon/circle checks \u2014 no HTTP call required.", "IGeofencingService"),
    ];
}

public record ServiceDefinition(string Title, string Href, string Description, string InterfaceName);
