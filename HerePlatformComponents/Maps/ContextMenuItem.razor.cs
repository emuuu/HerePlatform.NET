using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace HerePlatformComponents.Maps;

/// <summary>
/// A single item in a context menu. Must be placed inside a ContextMenuComponent.
/// </summary>
public partial class ContextMenuItem : ComponentBase
{
    /// <summary>
    /// Display text for the menu item.
    /// </summary>
    [Parameter, JsonIgnore]
    public string? Label { get; set; }

    /// <summary>
    /// Whether the menu item is disabled.
    /// </summary>
    [Parameter, JsonIgnore]
    public bool Disabled { get; set; }

    /// <summary>
    /// Custom data associated with this item.
    /// </summary>
    [Parameter, JsonIgnore]
    public object? Data { get; set; }

    [CascadingParameter]
    private ContextMenuComponent? ParentMenu { get; set; }

    protected override void OnInitialized()
    {
        ParentMenu?.RegisterItem(this);
        base.OnInitialized();
    }
}
