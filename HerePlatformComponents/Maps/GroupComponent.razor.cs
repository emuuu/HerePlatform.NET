using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps.Coordinates;
using HerePlatformComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HerePlatformComponents.Maps;

public partial class GroupComponent : IAsyncDisposable
{
    public GroupComponent()
    {
        _guid = Guid.NewGuid();
    }

    private bool _hasRendered = false;
    private bool _isDisposed;
    internal void MarkDisposed() => _isDisposed = true;
    private Guid _guid;

    public Guid Guid => _guid;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [CascadingParameter(Name = "HereMap")]
    private AdvancedHereMap MapRef { get; set; } = default!;

    [Parameter, JsonIgnore]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// If true, the group and all its children are visible.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Z-index for stacking order.
    /// </summary>
    [Parameter, JsonIgnore]
    public int? ZIndex { get; set; }

    /// <summary>
    /// Arbitrary data associated with this group.
    /// </summary>
    [Parameter, JsonIgnore]
    public object? Data { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MapRef.AddGroup(this);
            _hasRendered = true;
            await UpdateOptions();
            StateHasChanged(); // Re-render to show CascadingValue
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateOptions()
    {
        await Js.InvokeAsync<string>(
            JsInteropIdentifiers.UpdateGroupComponent,
            Guid,
            new GroupComponentOptions
            {
                Visible = Visible,
                ZIndex = ZIndex,
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
            parameters.DidParameterChange(Visible) ||
            parameters.DidParameterChange(ZIndex);

        await base.SetParametersAsync(parameters);

        if (optionsChanged)
        {
            await UpdateOptions();
        }
    }

    /// <summary>
    /// Adds a map object to this group by its GUID.
    /// </summary>
    public async Task AddObjectAsync(Guid objectGuid)
    {
        if (!_hasRendered) return;
        await Js.InvokeVoidAsync(JsInteropIdentifiers.GroupAddObjects, Guid, new[] { objectGuid });
    }

    /// <summary>
    /// Removes a map object from this group by its GUID.
    /// </summary>
    public async Task RemoveObjectAsync(Guid objectGuid)
    {
        if (!_hasRendered) return;
        await Js.InvokeVoidAsync(JsInteropIdentifiers.GroupRemoveObjects, Guid, new[] { objectGuid });
    }

    /// <summary>
    /// Gets the bounding box of all objects in this group.
    /// </summary>
    public async Task<GeoRect?> GetBoundsAsync()
    {
        if (!_hasRendered) return null;
        return await Js.InvokeAsync<GeoRect?>(JsInteropIdentifiers.GroupGetBounds, Guid);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        try
        {
            await Js.InvokeVoidAsync(JsInteropIdentifiers.DisposeGroupComponent, Guid);
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { }

        MapRef?.RemoveGroup(this);
        GC.SuppressFinalize(this);
    }

    internal readonly struct GroupComponentOptions
    {
        public bool? Visible { get; init; }
        public int? ZIndex { get; init; }
        public Guid? MapId { get; init; }
    }
}
