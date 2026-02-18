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
        new("Autosuggest", "services/autosuggest", "Autosuggest and autocomplete for addresses and places as the user types.", "IAutosuggestService"),
        new("Weather", "services/weather", "Current weather observations and forecasts for any location.", "IWeatherService"),
        new("Route Matching", "services/route-matching", "Match GPS traces to road segments for fleet tracking and analysis.", "IRouteMatchingService"),
        new("EV Charge Points", "services/ev-charge-points", "Find electric vehicle charging stations by location and connector type.", "IEvChargePointsService"),
        new("Map Image", "services/map-image", "Generate static map images with configurable style, zoom, and resolution.", "IMapImageService"),
        new("Intermodal Routing", "services/intermodal-routing", "Calculate routes combining public transit and walking segments.", "IIntermodalRoutingService"),
        new("Tour Planning", "services/tour-planning", "Optimize vehicle tours for deliveries and pickups with fleet constraints.", "ITourPlanningService"),
    ];
}

public record ServiceDefinition(string Title, string Href, string Description, string InterfaceName);
