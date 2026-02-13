using HerePlatformComponents.Maps.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

/// <summary>
/// Declarative context menu component for map right-click actions.
/// Place ContextMenuItem children inside to define menu items.
/// </summary>
public partial class ContextMenuComponent : IAsyncDisposable
{
    private readonly List<ContextMenuItem> _items = new();
    private DotNetObjectReference<ContextMenuComponent>? _selfRef;
    private MapPointerEventArgs? _lastContextMenuArgs;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    /// <summary>
    /// Menu items defined as child content.
    /// </summary>
    [Parameter, JsonIgnore]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Fired when a menu item is clicked.
    /// </summary>
    [Parameter, JsonIgnore]
    public EventCallback<ContextMenuEventArgs> OnItemClick { get; set; }

    internal void RegisterItem(ContextMenuItem item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
        }
    }

    protected override void OnInitialized()
    {
        _selfRef = DotNetObjectReference.Create(this);
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Register as the context menu handler on the parent map
            MapRef.RegisterContextMenuHandler(HandleContextMenu);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleContextMenu(MapPointerEventArgs args)
    {
        _lastContextMenuArgs = args;

        var jsItems = new List<object>();
        foreach (var item in _items)
        {
            jsItems.Add(new { label = item.Label ?? "", disabled = item.Disabled });
        }

        if (MapRef.MapId.HasValue)
        {
            await Js.InvokeVoidAsync(
                "blazorHerePlatform.objectManager.showContextMenu",
                MapRef.MapId.Value,
                jsItems.ToArray(),
                args.ViewportX,
                args.ViewportY,
                _selfRef);
        }
    }

    [JSInvokable]
    public async Task OnContextMenuItemClicked(int index, string label)
    {
        if (OnItemClick.HasDelegate && index >= 0 && index < _items.Count)
        {
            var item = _items[index];
            var eventArgs = new ContextMenuEventArgs
            {
                Position = _lastContextMenuArgs?.Position,
                ItemLabel = label,
                ItemData = item.Data,
                ViewportX = _lastContextMenuArgs?.ViewportX ?? 0,
                ViewportY = _lastContextMenuArgs?.ViewportY ?? 0
            };

            await OnItemClick.InvokeAsync(eventArgs);
        }
    }

    public async ValueTask DisposeAsync()
    {
        MapRef?.UnregisterContextMenuHandler();

        if (MapRef?.MapId.HasValue == true)
        {
            try
            {
                await Js.InvokeVoidAsync("blazorHerePlatform.objectManager.hideContextMenu", MapRef.MapId.Value);
            }
            catch (JSDisconnectedException) { }
            catch (InvalidOperationException) { }
        }

        _selfRef?.Dispose();
        GC.SuppressFinalize(this);
    }
}
