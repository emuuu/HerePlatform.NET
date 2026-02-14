using HerePlatformComponents.Maps.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public abstract class MapObjectComponentBase : ComponentBase, IAsyncDisposable
{
    private readonly Guid _guid = Guid.NewGuid();
    private bool _hasRendered;
    private bool _isDisposed;

    public Guid Guid => _guid;

    internal bool IsDisposed => _isDisposed;
    internal void MarkDisposed() => _isDisposed = true;

    protected bool HasRendered => _hasRendered;

    [Inject]
    protected IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    internal AdvancedHereMap MapRef { get; set; } = default!;

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnClick { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnDoubleClick { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnLongPress { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnContextMenu { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback OnContextMenuClose { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerDown { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerUp { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerMove { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerEnter { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapPointerEventArgs> OnPointerLeave { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback OnPointerCancel { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDragStart { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDrag { get; set; }

    [Parameter, JsonIgnore]
    public EventCallback<MapDragEventArgs> OnDragEnd { get; set; }

    protected bool HasBaseEventCallbacks =>
        OnClick.HasDelegate || OnDoubleClick.HasDelegate || OnLongPress.HasDelegate ||
        OnContextMenu.HasDelegate || OnContextMenuClose.HasDelegate ||
        OnPointerDown.HasDelegate || OnPointerUp.HasDelegate || OnPointerMove.HasDelegate ||
        OnPointerEnter.HasDelegate || OnPointerLeave.HasDelegate || OnPointerCancel.HasDelegate ||
        OnDragStart.HasDelegate || OnDrag.HasDelegate || OnDragEnd.HasDelegate;

    internal virtual bool HasAnyEventCallback => HasBaseEventCallbacks;

    internal async Task HandlePointerEvent(string eventName, MapPointerEventArgs args)
    {
        var callback = eventName switch
        {
            "tap" => OnClick,
            "dbltap" => OnDoubleClick,
            "longpress" => OnLongPress,
            "contextmenu" => OnContextMenu,
            "pointerdown" => OnPointerDown,
            "pointerup" => OnPointerUp,
            "pointermove" => OnPointerMove,
            "pointerenter" => OnPointerEnter,
            "pointerleave" => OnPointerLeave,
            _ => default
        };

        if (callback.HasDelegate)
            await callback.InvokeAsync(args);
    }

    internal async Task HandleContextMenuClose()
    {
        if (OnContextMenuClose.HasDelegate)
            await OnContextMenuClose.InvokeAsync();
    }

    internal async Task HandlePointerCancel()
    {
        if (OnPointerCancel.HasDelegate)
            await OnPointerCancel.InvokeAsync();
    }

    internal virtual async Task HandleDragEvent(string eventName, MapDragEventArgs args)
    {
        var callback = eventName switch
        {
            "dragstart" => OnDragStart,
            "drag" => OnDrag,
            "dragend" => OnDragEnd,
            _ => default
        };

        if (callback.HasDelegate)
            await callback.InvokeAsync(args);
    }

    protected abstract string JsDisposeFunction { get; }
    protected abstract Task RegisterWithMapAsync();
    protected abstract Task UnregisterFromMapAsync();
    protected abstract Task UpdateOptions();
    protected abstract bool CheckParameterChanges(ParameterView parameters);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RegisterWithMapAsync();
            _hasRendered = true;
            await UpdateOptions();
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

        var changed = CheckParameterChanges(parameters);
        await base.SetParametersAsync(parameters);
        if (changed)
            await UpdateOptions();
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            await Js.InvokeVoidAsync(JsDisposeFunction, Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        await UnregisterFromMapAsync();
        GC.SuppressFinalize(this);
    }
}
