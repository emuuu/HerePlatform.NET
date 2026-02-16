using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatform.Core.Services;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Services.Routing;

/// <summary>
/// Declarative route component. Calculates a route and renders it as a polyline on the map.
/// </summary>
public partial class RouteComponent : IAsyncDisposable
{
    public RouteComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    private bool _isDisposed;
    internal void MarkDisposed() => _isDisposed = true;
    private Guid _guid;
    private RoutingResult? _lastResult;
    private bool _isCalculating;

    public Guid Guid => _guid;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [Inject]
    private IRoutingService RoutingService { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// Route origin position.
    /// </summary>
    [Parameter]
    public LatLngLiteral Origin { get; set; }

    /// <summary>
    /// Route destination position.
    /// </summary>
    [Parameter]
    public LatLngLiteral Destination { get; set; }

    /// <summary>
    /// Optional via waypoints.
    /// </summary>
    [Parameter]
    public List<LatLngLiteral>? Via { get; set; }

    /// <summary>
    /// Transport mode (default: Car).
    /// </summary>
    [Parameter]
    public TransportMode TransportMode { get; set; } = TransportMode.Car;

    /// <summary>
    /// Routing optimization mode (default: Fast).
    /// </summary>
    [Parameter]
    public RoutingMode RoutingMode { get; set; } = RoutingMode.Fast;

    /// <summary>
    /// Stroke color for the route polyline.
    /// </summary>
    [Parameter]
    public string StrokeColor { get; set; } = "#0066FF";

    /// <summary>
    /// Line width for the route polyline.
    /// </summary>
    [Parameter]
    public double LineWidth { get; set; } = 5;

    /// <summary>
    /// Features to avoid in route.
    /// </summary>
    [Parameter]
    public RoutingAvoidFeature Avoid { get; set; } = RoutingAvoidFeature.None;

    /// <summary>
    /// If true, the route polyline is visible.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Fired when the route calculation completes.
    /// </summary>
    [Parameter]
    public EventCallback<RoutingResult> OnRouteCalculated { get; set; }

    /// <summary>
    /// Fired when a route calculation fails with an exception.
    /// </summary>
    [Parameter, System.Text.Json.Serialization.JsonIgnore]
    public EventCallback<Exception> OnError { get; set; }

    /// <summary>
    /// The most recent routing result.
    /// </summary>
    public RoutingResult? Result => _lastResult;

    /// <summary>
    /// Whether a route calculation is currently in progress.
    /// </summary>
    public bool IsCalculating => _isCalculating;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _hasRendered = true;
            await CalculateAndRender();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (!_hasRendered)
        {
            await base.SetParametersAsync(parameters);
            return;
        }

        var routeChanged =
            parameters.DidParameterChange(Origin) ||
            parameters.DidParameterChange(Destination) ||
            parameters.DidParameterChange(Via) ||
            parameters.DidParameterChange(TransportMode) ||
            parameters.DidParameterChange(RoutingMode) ||
            parameters.DidParameterChange(Avoid);

        var styleChanged =
            parameters.DidParameterChange(StrokeColor) ||
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(Visible);

        await base.SetParametersAsync(parameters);

        if (routeChanged)
        {
            await CalculateAndRender();
        }
        else if (styleChanged)
        {
            await UpdatePolylineStyle();
        }
    }

    private async Task CalculateAndRender()
    {
        if (_isDisposed) return;

        _isCalculating = true;

        try
        {
            var request = new RoutingRequest
            {
                Origin = Origin,
                Destination = Destination,
                Via = Via,
                TransportMode = TransportMode,
                RoutingMode = RoutingMode,
                Avoid = Avoid,
                ReturnPolyline = true
            };

            _lastResult = await RoutingService.CalculateRouteAsync(request);

            // Collect all decoded polyline points from all sections of the first route
            var path = new List<LatLngLiteral>();
            if (_lastResult?.Routes is { Count: > 0 })
            {
                var route = _lastResult.Routes[0];
                if (route.Sections != null)
                {
                    foreach (var section in route.Sections)
                    {
                        if (section.DecodedPolyline != null)
                        {
                            path.AddRange(section.DecodedPolyline);
                        }
                    }
                }
            }

            // Update or create the polyline
            if (path.Count > 0)
            {
                await Js.InvokeAsync<string>(
                    JsInteropIdentifiers.UpdatePolylineComponent,
                    _guid,
                    new
                    {
                        path,
                        strokeColor = StrokeColor,
                        lineWidth = LineWidth,
                        clickable = false,
                        visible = Visible,
                        mapId = MapRef.MapId
                    },
                    MapRef.CallbackRef);
            }

            if (OnRouteCalculated.HasDelegate)
            {
                await OnRouteCalculated.InvokeAsync(_lastResult);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[RouteComponent] Error: {ex.Message}");
            if (OnError.HasDelegate)
                await OnError.InvokeAsync(ex);
        }
        finally
        {
            _isCalculating = false;
        }
    }

    private async Task UpdatePolylineStyle()
    {
        if (_isDisposed || _lastResult?.Routes is not { Count: > 0 }) return;

        // Collect path again
        var path = new List<LatLngLiteral>();
        var route = _lastResult.Routes[0];
        if (route.Sections != null)
        {
            foreach (var section in route.Sections)
            {
                if (section.DecodedPolyline != null)
                    path.AddRange(section.DecodedPolyline);
            }
        }

        if (path.Count > 0)
        {
            await Js.InvokeAsync<string>(
                JsInteropIdentifiers.UpdatePolylineComponent,
                _guid,
                new
                {
                    path,
                    strokeColor = StrokeColor,
                    lineWidth = LineWidth,
                    clickable = false,
                    visible = Visible,
                    mapId = MapRef.MapId
                },
                MapRef.CallbackRef);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            await Js.InvokeVoidAsync(JsInteropIdentifiers.DisposePolylineComponent, _guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        GC.SuppressFinalize(this);
    }
}
