using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Standalone InfoBubble component. Renders an H.ui.InfoBubble at the given position.
/// ChildContent provides the HTML content. IsOpen is two-way bindable.
/// </summary>
public partial class InfoBubbleComponent : IAsyncDisposable
{
    public InfoBubbleComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    private bool _isDisposed;
    internal void MarkDisposed() => _isDisposed = true;
    private Guid _guid;

    public Guid Guid => _guid;

    internal string TemplateElementId => $"blz-ib-{_guid}";

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// HTML content for the InfoBubble. Read as static HTML.
    /// </summary>
    [Parameter, JsonIgnore]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Latitude of the InfoBubble anchor position.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lat { get; set; }

    /// <summary>
    /// Longitude of the InfoBubble anchor position.
    /// </summary>
    [Parameter, JsonIgnore]
    public double Lng { get; set; }

    /// <summary>
    /// Whether the InfoBubble is currently open. Two-way bindable via <c>@bind-IsOpen</c>.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Whether the map automatically pans to show the InfoBubble (default true).
    /// </summary>
    [Parameter, JsonIgnore]
    public bool AutoPan { get; set; } = true;

    /// <summary>
    /// Callback for two-way binding of <see cref="IsOpen"/>.
    /// Fired when the InfoBubble is closed by clicking the X button.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<bool> IsOpenChanged { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MapRef.AddInfoBubble(this);
            _hasRendered = true;
            if (IsOpen)
            {
                await UpdateOptions();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            "blazorHerePlatform.objectManager.updateInfoBubbleComponent",
            Guid,
            new InfoBubbleComponentOptions
            {
                Lat = Lat,
                Lng = Lng,
                IsOpen = IsOpen,
                AutoPan = AutoPan,
                MapId = MapRef.MapId,
                TemplateId = ChildContent is not null ? TemplateElementId : null,
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
            parameters.DidParameterChange(Lat) ||
            parameters.DidParameterChange(Lng) ||
            parameters.DidParameterChange(IsOpen) ||
            parameters.DidParameterChange(AutoPan);

        await base.SetParametersAsync(parameters);

        if (optionsChanged)
        {
            await UpdateOptions();
        }
    }

    /// <summary>
    /// Called from JS when the InfoBubble is closed by user clicking X.
    /// </summary>
    internal async Task HandleClosed()
    {
        if (IsOpen)
        {
            IsOpen = false;
            if (IsOpenChanged.HasDelegate)
                await IsOpenChanged.InvokeAsync(false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.disposeInfoBubbleComponent", Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        MapRef?.RemoveInfoBubble(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct InfoBubbleComponentOptions
    {
        public double Lat { get; init; }
        public double Lng { get; init; }
        public bool IsOpen { get; init; }
        public bool AutoPan { get; init; }
        public Guid? MapId { get; init; }
        public string? TemplateId { get; init; }
    }
}
