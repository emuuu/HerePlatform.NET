namespace HerePlatform.Blazor;

internal static class JsInteropIdentifiers
{
    private const string Prefix = "herePlatform.objectManager.";

    // Core object management
    internal const string CreateObject = Prefix + "createObject";
    internal const string CreateMultipleObject = Prefix + "createMultipleObject";
    internal const string DisposeObject = Prefix + "disposeObject";
    internal const string DisposeMultipleObjects = Prefix + "disposeMultipleObjects";
    internal const string Invoke = Prefix + "invoke";
    internal const string InvokeMultiple = Prefix + "invokeMultiple";
    internal const string InvokeWithReturnedObjectRef = Prefix + "invokeWithReturnedObjectRef";
    internal const string AddMultipleListeners = Prefix + "addMultipleListeners";
    internal const string ReadObjectPropertyValue = Prefix + "readObjectPropertyValue";
    internal const string ReadObjectPropertyValueWithReturnedObjectRef = Prefix + "readObjectPropertyValueWithReturnedObjectRef";
    internal const string WriteObjectPropertyValue = Prefix + "writeObjectPropertyValue";
    internal const string AddObjectToMap = Prefix + "addObjectToMap";
    internal const string RemoveObjectFromMap = Prefix + "removeObjectFromMap";
    internal const string AddObjectsToMap = Prefix + "addObjectsToMap";
    internal const string RemoveObjectsFromMap = Prefix + "removeObjectsFromMap";

    // Map lifecycle & interaction
    internal const string CanRenderMap = Prefix + "canRenderMap";
    internal const string InitMap = Prefix + "initMap";
    internal const string CreateHereMap = Prefix + "createHereMap";
    internal const string DisposeMap = Prefix + "disposeMap";
    internal const string SetupMapEvents = Prefix + "setupMapEvents";
    internal const string ResizeMap = Prefix + "resizeMap";
    internal const string GetViewBounds = Prefix + "getViewBounds";
    internal const string SetViewBounds = Prefix + "setViewBounds";
    internal const string SetBaseLayer = Prefix + "setBaseLayer";
    internal const string SetMapLookAt = Prefix + "setMapLookAt";
    internal const string GetMapLookAt = Prefix + "getMapLookAt";
    internal const string SetBehaviorFeatures = Prefix + "setBehaviorFeatures";
    internal const string CaptureMap = Prefix + "captureMap";
    internal const string SetViewportPadding = Prefix + "setViewportPadding";
    internal const string ZoomToBounds = Prefix + "zoomToBounds";
    internal const string ExportMapGeoJson = Prefix + "exportMapGeoJson";
    internal const string SetMinZoom = Prefix + "setMinZoom";
    internal const string SetMaxZoom = Prefix + "setMaxZoom";
    internal const string GetMinZoom = Prefix + "getMinZoom";
    internal const string GetMaxZoom = Prefix + "getMaxZoom";
    internal const string AddOverlayLayer = Prefix + "addOverlayLayer";
    internal const string RemoveOverlayLayer = Prefix + "removeOverlayLayer";

    // Marker
    internal const string UpdateMarkerComponent = Prefix + "updateMarkerComponent";
    internal const string DisposeMarkerComponent = Prefix + "disposeMarkerComponent";
    internal const string UpdateDomMarkerComponent = Prefix + "updateDomMarkerComponent";
    internal const string DisposeDomMarkerComponent = Prefix + "disposeDomMarkerComponent";

    // Shapes
    internal const string UpdatePolylineComponent = Prefix + "updatePolylineComponent";
    internal const string DisposePolylineComponent = Prefix + "disposePolylineComponent";
    internal const string UpdatePolygonComponent = Prefix + "updatePolygonComponent";
    internal const string DisposePolygonComponent = Prefix + "disposePolygonComponent";
    internal const string UpdateCircleComponent = Prefix + "updateCircleComponent";
    internal const string DisposeCircleComponent = Prefix + "disposeCircleComponent";
    internal const string UpdateRectComponent = Prefix + "updateRectComponent";
    internal const string DisposeRectComponent = Prefix + "disposeRectComponent";

    // Group
    internal const string UpdateGroupComponent = Prefix + "updateGroupComponent";
    internal const string DisposeGroupComponent = Prefix + "disposeGroupComponent";
    internal const string GroupAddObjects = Prefix + "groupAddObjects";
    internal const string GroupRemoveObjects = Prefix + "groupRemoveObjects";
    internal const string GroupGetBounds = Prefix + "groupGetBounds";

    // InfoBubble
    internal const string UpdateInfoBubbleComponent = Prefix + "updateInfoBubbleComponent";
    internal const string DisposeInfoBubbleComponent = Prefix + "disposeInfoBubbleComponent";
    internal const string AddInfoBubble = Prefix + "addInfoBubble";
    internal const string RemoveInfoBubble = Prefix + "removeInfoBubble";

    // Context menu
    internal const string ShowContextMenu = Prefix + "showContextMenu";
    internal const string HideContextMenu = Prefix + "hideContextMenu";

    // UI controls
    internal const string UpdateZoomRectangle = Prefix + "updateZoomRectangle";
    internal const string DisposeZoomRectangle = Prefix + "disposeZoomRectangle";
    internal const string UpdateDistanceMeasurement = Prefix + "updateDistanceMeasurement";
    internal const string DisposeDistanceMeasurement = Prefix + "disposeDistanceMeasurement";
    internal const string UpdateOverviewMap = Prefix + "updateOverviewMap";
    internal const string DisposeOverviewMap = Prefix + "disposeOverviewMap";

    // Data readers
    internal const string UpdateGeoJsonReaderComponent = Prefix + "updateGeoJsonReaderComponent";
    internal const string DisposeGeoJsonReaderComponent = Prefix + "disposeGeoJsonReaderComponent";
    internal const string UpdateKmlReaderComponent = Prefix + "updateKmlReaderComponent";
    internal const string DisposeKmlReaderComponent = Prefix + "disposeKmlReaderComponent";
    internal const string UpdateHeatmapComponent = Prefix + "updateHeatmapComponent";
    internal const string DisposeHeatmapComponent = Prefix + "disposeHeatmapComponent";

    // Layers
    internal const string UpdateImageOverlayComponent = Prefix + "updateImageOverlayComponent";
    internal const string DisposeImageOverlayComponent = Prefix + "disposeImageOverlayComponent";
    internal const string UpdateCustomTileLayer = Prefix + "updateCustomTileLayer";
    internal const string DisposeCustomTileLayer = Prefix + "disposeCustomTileLayer";

    // Clustering
    internal const string UpdateMarkerClusterComponent = Prefix + "updateMarkerClusterComponent";
    internal const string DisposeMarkerClusterComponent = Prefix + "disposeMarkerClusterComponent";

    // Services
    internal const string Geocode = Prefix + "geocode";
    internal const string ReverseGeocode = Prefix + "reverseGeocode";
    internal const string CalculateRoute = Prefix + "calculateRoute";
    internal const string GetTrafficIncidents = Prefix + "getTrafficIncidents";
    internal const string GetTrafficFlow = Prefix + "getTrafficFlow";
    internal const string GetTransitDepartures = Prefix + "getTransitDepartures";
    internal const string SearchTransitStations = Prefix + "searchTransitStations";
    internal const string DiscoverPlaces = Prefix + "discoverPlaces";
    internal const string BrowsePlaces = Prefix + "browsePlaces";
    internal const string LookupPlace = Prefix + "lookupPlace";
    internal const string CalculateIsoline = Prefix + "calculateIsoline";
    internal const string CalculateMatrix = Prefix + "calculateMatrix";
    internal const string OptimizeWaypointSequence = Prefix + "optimizeWaypointSequence";

    // Search
    internal const string Autosuggest = Prefix + "autosuggest";
    internal const string DisposeAutosuggest = Prefix + "disposeAutosuggest";
}
