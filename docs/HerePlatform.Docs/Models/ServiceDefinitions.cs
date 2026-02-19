namespace HerePlatform.Docs.Models;

public static class ServiceDefinitions
{
    public static readonly IReadOnlyList<ServiceDefinition> All =
    [
        new("Geocoding", "rest-api/geocoding", "Forward and reverse geocoding — convert addresses to coordinates and back.", "IGeocodingService"),
        new("Routing", "rest-api/routing", "Calculate routes with support for car, truck, pedestrian, bicycle, and EV options.", "IRoutingService"),
        new("Places", "rest-api/places", "Discover, browse, and look up places by text search, category, or place ID.", "IPlacesService"),
        new("Isoline", "rest-api/isoline", "Calculate reachable areas (isochrones/isodistances) from a center point.", "IIsolineService"),
        new("Matrix Routing", "rest-api/matrix-routing", "Compute travel time and distance matrices between multiple origins and destinations.", "IMatrixRoutingService"),
        new("Traffic", "rest-api/traffic", "Get real-time traffic incidents and flow data within a bounding box.", "ITrafficService"),
        new("Public Transit", "rest-api/public-transit", "Search for transit stations and get live departure information.", "IPublicTransitService"),
        new("Waypoint Sequence", "rest-api/waypoint-sequence", "Optimize the order of waypoints for minimum travel time or distance.", "IWaypointSequenceService"),
        new("Geofencing", "rest-api/geofencing", "Client-side point-in-polygon/circle checks — no HTTP call required.", "IGeofencingService"),
        new("Autosuggest", "rest-api/autosuggest", "Autosuggest and autocomplete for addresses and places as the user types.", "IAutosuggestService"),
        new("Weather", "rest-api/weather", "Current weather observations and forecasts for any location.", "IWeatherService"),
        new("Route Matching", "rest-api/route-matching", "Match GPS traces to road segments for fleet tracking and analysis.", "IRouteMatchingService"),
        new("EV Charge Points", "rest-api/ev-charge-points", "Find electric vehicle charging stations by location and connector type.", "IEvChargePointsService"),
        new("Map Image", "rest-api/map-image", "Generate static map images with configurable style, zoom, and resolution.", "IMapImageService"),
        new("Intermodal Routing", "rest-api/intermodal-routing", "Calculate routes combining public transit and walking segments.", "IIntermodalRoutingService"),
        new("Tour Planning", "rest-api/tour-planning", "Optimize vehicle tours for deliveries and pickups with fleet constraints.", "ITourPlanningService"),
    ];
}

public record ServiceDefinition(string Title, string Href, string Description, string InterfaceName);
