using HerePlatform.Core.Search;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HerePlatform.Blazor.Maps.Search;

public partial class HereAutosuggest : IAsyncDisposable
{
    private readonly Guid _guid = Guid.NewGuid();
    private DotNetObjectReference<HereAutosuggest>? _callbackRef;
    private CancellationTokenSource? _debounceCts;
    private List<AutosuggestItem> _items = new();
    private bool _isOpen;
    private int _activeIndex = -1;
    private bool _isDisposed;
    private bool _platformInitialized;
    private ElementReference _inputRef;
    private ElementReference _rootRef;
    private bool _keyboardAttached;

    [Inject]
    private IJSRuntime Js { get; set; } = default!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?>? UserAttributes { get; set; }

    /// <summary>
    /// Placeholder text for the input field.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Additional CSS class applied to the wrapper div.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Search configuration options (language, country filter, limit, bias location).
    /// </summary>
    [Parameter]
    public AutosuggestOptions? Options { get; set; }

    /// <summary>
    /// Disables the input when true.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Current text value of the input (two-way bindable via @bind-Value).
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Callback for two-way binding of Value.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Fires when the user selects a suggestion from the dropdown.
    /// </summary>
    [Parameter]
    public EventCallback<AutosuggestItem> OnItemSelected { get; set; }

    /// <summary>
    /// Fires when the input is cleared.
    /// </summary>
    [Parameter]
    public EventCallback OnCleared { get; set; }

    /// <summary>
    /// Fires when an error occurs during autosuggest, e.g. an authentication failure,
    /// a rejected request (HTTP 4xx from the HERE API), or invalid <c>Options</c>.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    /// <summary>
    /// Minimum number of characters before triggering a search. Default: 3.
    /// </summary>
    [Parameter]
    public int MinChars { get; set; } = 3;

    /// <summary>
    /// Debounce delay in milliseconds before triggering a search. Default: 300.
    /// </summary>
    [Parameter]
    public int DebounceMs { get; set; } = 300;

    /// <summary>
    /// Predefined design variant for the component. Default: <see cref="AutosuggestDesign.Default"/>.
    /// </summary>
    [Parameter]
    public AutosuggestDesign Design { get; set; } = AutosuggestDesign.Default;

    /// <summary>
    /// Custom template for rendering each suggestion item. When set, replaces the default item content
    /// inside the <c>&lt;li&gt;</c> shell (click handler and active class are preserved).
    /// Ignored when <see cref="SuggestionListTemplate"/> is set.
    /// </summary>
    [Parameter]
    public RenderFragment<AutosuggestItem>? SuggestionItemTemplate { get; set; }

    /// <summary>
    /// Custom template for rendering the entire suggestion dropdown.
    /// When set, replaces the default <c>&lt;ul&gt;</c> list completely.
    /// Use <see cref="AutosuggestListContext.SelectItem"/> to wire up item selection.
    /// </summary>
    [Parameter]
    public RenderFragment<AutosuggestListContext>? SuggestionListTemplate { get; set; }

    /// <summary>
    /// Custom template for rendering the input area. The <see cref="AutosuggestInputContext"/> provides
    /// an <c>InputAttributes</c> dictionary that must be splatted onto the custom <c>&lt;input&gt;</c> element
    /// (<c>@attributes="context.InputAttributes"</c>) — nothing else is required. Keyboard defaults are
    /// handled by the component: splatting also applies the <c>data-here-autosuggest-input</c> marker that
    /// lets the component suppress the browser default for Enter and ArrowUp/ArrowDown — and only those —
    /// while the dropdown is open. Do NOT apply <c>@onkeydown:preventDefault</c> in a custom template.
    /// </summary>
    [Parameter]
    public RenderFragment<AutosuggestInputContext>? InputTemplate { get; set; }

    private string VariantCssClass => Design switch
    {
        AutosuggestDesign.Compact => "here-autosuggest-compact",
        AutosuggestDesign.Filled => "here-autosuggest-filled",
        AutosuggestDesign.Outlined => "here-autosuggest-outlined",
        AutosuggestDesign.Rounded => "here-autosuggest-rounded",
        _ => "here-autosuggest-default"
    };

    private string InputVariantCssClass => Design switch
    {
        AutosuggestDesign.Compact => "here-input-compact",
        AutosuggestDesign.Filled => "here-input-filled",
        AutosuggestDesign.Outlined => "here-input-outlined",
        AutosuggestDesign.Rounded => "here-input-rounded",
        _ => ""
    };

    private string DropdownVariantCssClass => Design switch
    {
        AutosuggestDesign.Compact => "here-dropdown-compact",
        AutosuggestDesign.Rounded => "here-dropdown-rounded",
        _ => ""
    };

    private string ItemVariantCssClass => Design switch
    {
        AutosuggestDesign.Compact => "here-item-compact",
        AutosuggestDesign.Rounded => "here-item-rounded",
        _ => ""
    };

    // Rendered as data-here-autosuggest-open on the wrapper div, where the JS keydown listener reads it.
    // Blazor stays the single source of truth for the dropdown state — nothing is mirrored in JS.
    private bool IsDropdownOpen => _isOpen && _items.Count > 0;

    private AutosuggestInputContext BuildInputContext() => new()
    {
        Value = Value,
        Placeholder = Placeholder,
        Disabled = Disabled,
        InputAttributes = new Dictionary<string, object>
        {
            ["value"] = Value ?? "",
            ["placeholder"] = Placeholder ?? "",
            ["disabled"] = Disabled,
            ["autocomplete"] = "off",
            // Marker that identifies the input to the JS keydown listener — splatting
            // InputAttributes is all a custom InputTemplate has to do.
            ["data-here-autosuggest-input"] = "",
            ["oninput"] = EventCallback.Factory.Create<ChangeEventArgs>(this, OnInput),
            ["onkeydown"] = EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDown)
        }
    };

    private AutosuggestListContext BuildListContext() => new()
    {
        Items = _items.AsReadOnly(),
        ActiveIndex = _activeIndex,
        SelectItem = item => InvokeAsync(() => SelectItem(item))
    };

    protected override void OnInitialized()
    {
        _callbackRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Not tied to firstRender: if the interop call fails because the circuit is down, the next
        // render retries it — otherwise the component would stay without a listener for good.
        // attachAutosuggestKeyboard is idempotent, so a repeated call is harmless.
        if (_keyboardAttached || _isDisposed) return;

        // Blazor's @onkeydown:preventDefault is a static per-event/per-element flag, so it cannot
        // suppress the default for single keys. A JS keydown listener delegated on the wrapper does it
        // key-selectively: only Enter (implicit form submit) and ArrowUp/ArrowDown (caret jump), and
        // only while the list is open.
        try
        {
            await Js.InvokeVoidAsync(JsInteropIdentifiers.AttachAutosuggestKeyboard, _guid, _rootRef);
            _keyboardAttached = true;
        }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }
    }

    private async Task OnInput(ChangeEventArgs e)
    {
        var text = e.Value?.ToString() ?? "";
        Value = text;

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(text);

        if (string.IsNullOrWhiteSpace(text))
        {
            CloseDropdown();
            if (OnCleared.HasDelegate)
                await OnCleared.InvokeAsync();
            return;
        }

        if (text.Length < MinChars)
        {
            CloseDropdown();
            return;
        }

        // Debounce
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        try
        {
            await Task.Delay(DebounceMs, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (token.IsCancellationRequested) return;

        await SearchAsync(text);
    }

    private async Task EnsurePlatformAsync()
    {
        if (_platformInitialized) return;

        var keyService = ServiceProvider.GetService<IHerePlatformKeyService>();
        if (keyService is null) return;

        bool isReady = false;
        try
        {
            isReady = await Js.InvokeAsync<bool>(JsInteropIdentifiers.CanRenderMap);
        }
        catch { /* JS not available yet */ }

        if (!keyService.IsApiInitialized || !isReady)
        {
            keyService.MarkApiInitialized();
            var apiOptions = await keyService.GetApiOptions();
            await Js.InvokeVoidAsync(JsInteropIdentifiers.InitMap, apiOptions);
        }

        _platformInitialized = true;
    }

    private async Task SearchAsync(string query)
    {
        var options = Options ?? new AutosuggestOptions();

        try
        {
            options.EnsureValidForAutosuggest();
        }
        catch (InvalidOperationException ex)
        {
            // Mirror the JS error path, which clears results via OnAutosuggestResults([]) —
            // stale suggestions from a previous search must not stay visible.
            CloseDropdown();
            StateHasChanged();
            await OnAutosuggestError(ex.Message);
            return;
        }

        await EnsurePlatformAsync();

        // 'at' and 'in=circle/bbox' are mutually exclusive per the HERE API —
        // when In carries its own spatial context, at must be omitted.
        var sendAt = options.At.HasValue && !options.InProvidesSpatialContext();

        var jsOptions = new
        {
            limit = options.Limit,
            lang = options.Lang,
            @in = options.In,
            show = options.Show,
            at = sendAt
                ? new { lat = options.At!.Value.Lat, lng = options.At.Value.Lng }
                : (object?)null
        };

        await Js.InvokeVoidAsync(
            JsInteropIdentifiers.Autosuggest,
            _guid,
            query,
            jsOptions,
            _callbackRef);
    }

    [JSInvokable]
    public void OnAutosuggestResults(List<AutosuggestItem> items)
    {
        _items = items ?? new List<AutosuggestItem>();
        _activeIndex = -1;
        _isOpen = _items.Count > 0;
        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnAutosuggestError(string message)
    {
        if (OnError.HasDelegate)
            await OnError.InvokeAsync(message);
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowDown":
                if (_isOpen && _items.Count > 0)
                    _activeIndex = (_activeIndex + 1) % _items.Count;
                break;

            case "ArrowUp":
                if (_isOpen && _items.Count > 0)
                    _activeIndex = _activeIndex <= 0 ? _items.Count - 1 : _activeIndex - 1;
                break;

            case "Enter":
                // With an open list, Enter always picks a suggestion: the active one if the user
                // navigated with arrow keys, otherwise the first result.
                if (_isOpen && _items.Count > 0)
                {
                    var index = _activeIndex >= 0 && _activeIndex < _items.Count ? _activeIndex : 0;
                    await SelectItem(_items[index]);
                }
                break;

            case "Escape":
                CloseDropdown();
                break;
        }
    }

    private async Task SelectItem(AutosuggestItem item)
    {
        Value = item.Address?.Label ?? item.Title;
        CloseDropdown();

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);

        if (OnItemSelected.HasDelegate)
            await OnItemSelected.InvokeAsync(item);
    }

    private async Task OnFocusOut()
    {
        // Small delay to allow click on dropdown item to fire first
        await Task.Delay(200);

        if (_isOpen)
        {
            CloseDropdown();
            StateHasChanged();
        }
    }

    private void CloseDropdown()
    {
        _isOpen = false;
        _items.Clear();
        _activeIndex = -1;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _debounceCts?.Cancel();
        _debounceCts?.Dispose();

        try
        {
            await Js.InvokeVoidAsync(JsInteropIdentifiers.DisposeAutosuggest, _guid);
        }
        catch (JSDisconnectedException) { }
        catch (OperationCanceledException) { }

        _callbackRef?.Dispose();
        GC.SuppressFinalize(this);
    }
}
