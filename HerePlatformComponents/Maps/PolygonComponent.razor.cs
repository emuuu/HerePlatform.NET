using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class PolygonComponent : IAsyncDisposable
{
    public PolygonComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    internal bool IsDisposed = false;
    private Guid _guid;

    public Guid Guid => _guid;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// Path defining the polygon boundary.
    /// </summary>
    [Parameter, JsonIgnore]
    public List<LatLngLiteral>? Path { get; set; }

    /// <summary>
    /// Stroke color in CSS format.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? StrokeColor { get; set; }

    /// <summary>
    /// Fill color in CSS format.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? FillColor { get; set; }

    /// <summary>
    /// Line width in pixels.
    /// </summary>
    [Parameter, JsonIgnore]
    public double? LineWidth { get; set; }

    /// <summary>
    /// If true, the polygon is clickable.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Clickable { get; set; }

    /// <summary>
    /// If true, the polygon is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Fired when the polygon is tapped/clicked.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback OnClick { get; set; }

    internal async Task Click()
    {
        await OnClick.InvokeAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MapRef.AddPolygon(this);
            _hasRendered = true;
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updatePolygonComponent",
            Guid,
            new PolygonComponentOptions
            {
                Path = Path,
                StrokeColor = StrokeColor,
                FillColor = FillColor,
                LineWidth = LineWidth,
                Clickable = Clickable,
                Visible = Visible,
                MapId = MapRef.MapId,
            },
            MapRef.CallbackRef);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (!_hasRendered)
        {
            await base.SetParametersAsync(parameters);
            return;
        }

        var optionsChanged =
            parameters.DidParameterChange(Path) ||
            parameters.DidParameterChange(StrokeColor) ||
            parameters.DidParameterChange(FillColor) ||
            parameters.DidParameterChange(LineWidth) ||
            parameters.DidParameterChange(Clickable) ||
            parameters.DidParameterChange(Visible);

        await base.SetParametersAsync(parameters);

        if (optionsChanged)
        {
            await UpdateOptions();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (IsDisposed) return;
        IsDisposed = true;
        await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposePolygonComponent", Guid);
        MapRef.RemovePolygon(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct PolygonComponentOptions
    {
        public List<LatLngLiteral>? Path { get; init; }
        public string? StrokeColor { get; init; }
        public string? FillColor { get; init; }
        public double? LineWidth { get; init; }
        public bool? Clickable { get; init; }
        public bool? Visible { get; init; }
        public Guid? MapId { get; init; }
    }
}
