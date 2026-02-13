using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps.Data;

/// <summary>
/// Declarative KML data import component.
/// Requires HereApiLoadOptions.LoadData = true.
/// </summary>
public partial class KmlReaderComponent : IAsyncDisposable
{
    public KmlReaderComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    private bool _isDisposed = false;
    private Guid _guid;

    public Guid Guid => _guid;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// URL of the KML file to load.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? Url { get; set; }

    /// <summary>
    /// Default style to apply to loaded objects.
    /// </summary>
    [Parameter, JsonIgnore]
    public StyleOptions? DefaultStyle { get; set; }

    /// <summary>
    /// Whether the loaded layer is visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Fired when the KML data has been loaded and parsed.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<GeoJsonLoadedEventArgs> OnLoaded { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _hasRendered = true;
            MapRef.AddKmlReader(this);
            await UpdateOptions();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    internal async Task HandleLoaded(GeoJsonLoadedEventArgs args)
    {
        if (OnLoaded.HasDelegate)
            await OnLoaded.InvokeAsync(args);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateKmlReaderComponent",
            Guid,
            new KmlReaderComponentOptions
            {
                Url = Url,
                DefaultStyle = DefaultStyle,
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
            parameters.DidParameterChange(Url) ||
            parameters.DidParameterChange(Visible);

        await base.SetParametersAsync(parameters);

        if (optionsChanged)
        {
            await UpdateOptions();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        MapRef?.RemoveKmlReader(this);

        try
        {
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeKmlReaderComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        GC.SuppressFinalize(this);
    }

    internal readonly struct KmlReaderComponentOptions
    {
        public string? Url { get; init; }
        public StyleOptions? DefaultStyle { get; init; }
        public bool Visible { get; init; }
        public Guid? MapId { get; init; }
    }
}
