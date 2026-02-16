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
using HerePlatform.Core.WaypointSequence;

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

// WaypointSequence
[assembly: TypeForwardedTo(typeof(WaypointSequenceRequest))]
[assembly: TypeForwardedTo(typeof(WaypointSequenceResult))]

// Utilities
[assembly: TypeForwardedTo(typeof(FlexiblePolyline))]
[assembly: TypeForwardedTo(typeof(GeoJsonExporter))]
[assembly: TypeForwardedTo(typeof(WktParser))]

// Exceptions
[assembly: TypeForwardedTo(typeof(HereApiAuthenticationException))]

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
