using System.Runtime.CompilerServices;
using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Geocoding;
using HerePlatform.Core.Geofencing;
using HerePlatform.Core.Isoline;
using HerePlatform.Core.MatrixRouting;
using HerePlatform.Core.Places;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Search;
using HerePlatform.Core.Serialization;
using HerePlatform.Core.Services;
using HerePlatform.Core.Traffic;
using HerePlatform.Core.Transit;
using HerePlatform.Core.Exceptions;
using HerePlatform.Core.Utilities;
using HerePlatform.Core.RouteMatching;
using HerePlatform.Core.Weather;
using HerePlatform.Core.WaypointSequence;
using HerePlatform.Core.EvChargePoints;
using HerePlatform.Core.MapImage;
using HerePlatform.Core.IntermodalRouting;
using HerePlatform.Core.TourPlanning;

// Coordinates
[assembly: TypeForwardedTo(typeof(LatLngLiteral))]
[assembly: TypeForwardedTo(typeof(GeoPoint))]
[assembly: TypeForwardedTo(typeof(GeoRect))]

// Serialization
[assembly: TypeForwardedTo(typeof(JsonStringEnumConverterEx<>))]

// Routing
[assembly: TypeForwardedTo(typeof(TransportMode))]
[assembly: TypeForwardedTo(typeof(RoutingMode))]
[assembly: TypeForwardedTo(typeof(RoutingAvoidFeature))]
[assembly: TypeForwardedTo(typeof(TunnelCategory))]
[assembly: TypeForwardedTo(typeof(HazardousGoods))]
[assembly: TypeForwardedTo(typeof(TruckOptions))]
[assembly: TypeForwardedTo(typeof(EvOptions))]
[assembly: TypeForwardedTo(typeof(TurnInstruction))]
[assembly: TypeForwardedTo(typeof(RoutingRequest))]
[assembly: TypeForwardedTo(typeof(RoutingResult))]
[assembly: TypeForwardedTo(typeof(Route))]
[assembly: TypeForwardedTo(typeof(RouteSection))]
[assembly: TypeForwardedTo(typeof(RouteSummary))]

// Geocoding
[assembly: TypeForwardedTo(typeof(GeocodeOptions))]
[assembly: TypeForwardedTo(typeof(GeocodeResult))]
[assembly: TypeForwardedTo(typeof(GeocodeItem))]

// Search
[assembly: TypeForwardedTo(typeof(AutosuggestAddress))]
[assembly: TypeForwardedTo(typeof(AutosuggestItem))]
[assembly: TypeForwardedTo(typeof(AutosuggestHighlights))]
[assembly: TypeForwardedTo(typeof(AutosuggestHighlightRange))]
[assembly: TypeForwardedTo(typeof(AutosuggestOptions))]
[assembly: TypeForwardedTo(typeof(AutosuggestResult))]
[assembly: TypeForwardedTo(typeof(AutocompleteResult))]

// MatrixRouting
[assembly: TypeForwardedTo(typeof(MatrixRoutingRequest))]
[assembly: TypeForwardedTo(typeof(MatrixRoutingResult))]
[assembly: TypeForwardedTo(typeof(MatrixEntry))]

// Isoline
[assembly: TypeForwardedTo(typeof(IsolineRangeType))]
[assembly: TypeForwardedTo(typeof(IsolineRequest))]
[assembly: TypeForwardedTo(typeof(IsolineResult))]
[assembly: TypeForwardedTo(typeof(IsolinePolygon))]

// Traffic
[assembly: TypeForwardedTo(typeof(TrafficFlowItem))]
[assembly: TypeForwardedTo(typeof(TrafficFlowResult))]
[assembly: TypeForwardedTo(typeof(TrafficIncident))]
[assembly: TypeForwardedTo(typeof(TrafficIncidentsResult))]

// Transit
[assembly: TypeForwardedTo(typeof(TransitDeparture))]
[assembly: TypeForwardedTo(typeof(TransitDeparturesResult))]
[assembly: TypeForwardedTo(typeof(TransitStation))]
[assembly: TypeForwardedTo(typeof(TransitStationsResult))]

// Places
[assembly: TypeForwardedTo(typeof(PlacesRequest))]
[assembly: TypeForwardedTo(typeof(PlacesResult))]
[assembly: TypeForwardedTo(typeof(PlaceItem))]
[assembly: TypeForwardedTo(typeof(PlaceContact))]

// Geofencing
[assembly: TypeForwardedTo(typeof(GeofenceZone))]
[assembly: TypeForwardedTo(typeof(GeofenceCheckResult))]

// RouteMatching
[assembly: TypeForwardedTo(typeof(TracePoint))]
[assembly: TypeForwardedTo(typeof(RouteMatchingRequest))]
[assembly: TypeForwardedTo(typeof(MatchedLink))]
[assembly: TypeForwardedTo(typeof(RouteMatchingResult))]

// Weather
[assembly: TypeForwardedTo(typeof(WeatherProduct))]
[assembly: TypeForwardedTo(typeof(WeatherRequest))]
[assembly: TypeForwardedTo(typeof(WeatherObservation))]
[assembly: TypeForwardedTo(typeof(WeatherForecast))]
[assembly: TypeForwardedTo(typeof(WeatherResult))]

// WaypointSequence
[assembly: TypeForwardedTo(typeof(WaypointSequenceRequest))]
[assembly: TypeForwardedTo(typeof(WaypointSequenceResult))]

// EvChargePoints
[assembly: TypeForwardedTo(typeof(ConnectorType))]
[assembly: TypeForwardedTo(typeof(EvConnector))]
[assembly: TypeForwardedTo(typeof(EvStation))]
[assembly: TypeForwardedTo(typeof(EvChargePointsResult))]
[assembly: TypeForwardedTo(typeof(EvChargePointsRequest))]

// MapImage
[assembly: TypeForwardedTo(typeof(MapImageFormat))]
[assembly: TypeForwardedTo(typeof(MapImageStyle))]
[assembly: TypeForwardedTo(typeof(MapImageRequest))]

// IntermodalRouting
[assembly: TypeForwardedTo(typeof(IntermodalRoutingRequest))]
[assembly: TypeForwardedTo(typeof(IntermodalRoutingResult))]
[assembly: TypeForwardedTo(typeof(IntermodalRoute))]
[assembly: TypeForwardedTo(typeof(IntermodalSection))]
[assembly: TypeForwardedTo(typeof(IntermodalPlace))]
[assembly: TypeForwardedTo(typeof(IntermodalSummary))]
[assembly: TypeForwardedTo(typeof(IntermodalTransport))]

// TourPlanning
[assembly: TypeForwardedTo(typeof(TourPlanningProblem))]
[assembly: TypeForwardedTo(typeof(TourPlan))]
[assembly: TypeForwardedTo(typeof(TourJob))]
[assembly: TypeForwardedTo(typeof(TourJobPlaces))]
[assembly: TypeForwardedTo(typeof(TourJobPlace))]
[assembly: TypeForwardedTo(typeof(TourFleet))]
[assembly: TypeForwardedTo(typeof(TourVehicleType))]
[assembly: TypeForwardedTo(typeof(TourVehicleCosts))]
[assembly: TypeForwardedTo(typeof(TourVehicleShift))]
[assembly: TypeForwardedTo(typeof(TourShiftEnd))]
[assembly: TypeForwardedTo(typeof(TourPlanningResult))]
[assembly: TypeForwardedTo(typeof(Tour))]
[assembly: TypeForwardedTo(typeof(TourStop))]
[assembly: TypeForwardedTo(typeof(TourActivity))]
[assembly: TypeForwardedTo(typeof(TourStatistic))]

// Utilities
[assembly: TypeForwardedTo(typeof(FlexiblePolyline))]
[assembly: TypeForwardedTo(typeof(GeoJsonExporter))]
[assembly: TypeForwardedTo(typeof(WktParser))]

// Exceptions
[assembly: TypeForwardedTo(typeof(HereApiException))]
[assembly: TypeForwardedTo(typeof(HereApiAuthenticationException))]

// Service attributes
[assembly: TypeForwardedTo(typeof(HereApiAttribute))]

// Service interfaces
[assembly: TypeForwardedTo(typeof(IRoutingService))]
[assembly: TypeForwardedTo(typeof(IGeocodingService))]
[assembly: TypeForwardedTo(typeof(IIsolineService))]
[assembly: TypeForwardedTo(typeof(IMatrixRoutingService))]
[assembly: TypeForwardedTo(typeof(IPlacesService))]
[assembly: TypeForwardedTo(typeof(IPublicTransitService))]
[assembly: TypeForwardedTo(typeof(ITrafficService))]
[assembly: TypeForwardedTo(typeof(IWaypointSequenceService))]
[assembly: TypeForwardedTo(typeof(IGeofencingService))]
[assembly: TypeForwardedTo(typeof(IAutosuggestService))]
[assembly: TypeForwardedTo(typeof(IWeatherService))]
[assembly: TypeForwardedTo(typeof(IRouteMatchingService))]
[assembly: TypeForwardedTo(typeof(IEvChargePointsService))]
[assembly: TypeForwardedTo(typeof(IMapImageService))]
[assembly: TypeForwardedTo(typeof(IIntermodalRoutingService))]
[assembly: TypeForwardedTo(typeof(ITourPlanningService))]
