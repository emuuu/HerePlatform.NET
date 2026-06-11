using AngleSharp.Dom;
using Bunit;
using HerePlatform.Core.Search;
using HerePlatform.Blazor.Maps.Search;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace HerePlatform.Blazor.Tests.Search;

public class HereAutosuggestInteractionTests : BunitTestBase
{
    private static List<AutosuggestItem> CreateTestItems() =>
    [
        new() { Title = "Berlin", Id = "1", Address = new AutosuggestAddress { Label = "Berlin, Germany" } },
        new() { Title = "Bern", Id = "2", Address = new AutosuggestAddress { Label = "Bern, Switzerland" } },
        new() { Title = "Bremen", Id = "3", Address = new AutosuggestAddress { Label = "Bremen, Germany" } }
    ];

    private void InjectResults(IRenderedComponent<HereAutosuggest> cut, List<AutosuggestItem>? items = null)
    {
        // OnAutosuggestResults calls StateHasChanged, which must run on the renderer dispatcher
        cut.InvokeAsync(() => cut.Instance.OnAutosuggestResults(items ?? CreateTestItems()));
    }

    [Test]
    public void OnAutosuggestResults_Opens_Dropdown_With_Items()
    {
        var cut = Render<HereAutosuggest>();

        InjectResults(cut);

        var items = cut.FindAll("li.here-autosuggest-item");
        Assert.That(items, Has.Count.EqualTo(3));
    }

    [Test]
    public void ArrowDown_Moves_Active_Index()
    {
        var cut = Render<HereAutosuggest>();
        InjectResults(cut);

        cut.Find("input").KeyDown(Key.Down);

        var items = cut.FindAll("li.here-autosuggest-item");
        Assert.That(items[0].ClassList, Does.Contain("active"));
        Assert.That(items[1].ClassList, Does.Not.Contain("active"));
    }

    [Test]
    public void ArrowUp_Wraps_To_Last_Item()
    {
        var cut = Render<HereAutosuggest>();
        InjectResults(cut);

        // First ArrowUp from -1 should wrap to last item
        cut.Find("input").KeyDown(Key.Up);

        var items = cut.FindAll("li.here-autosuggest-item");
        Assert.That(items[2].ClassList, Does.Contain("active"));
    }

    [Test]
    public void ArrowDown_Wraps_Around()
    {
        var cut = Render<HereAutosuggest>();
        InjectResults(cut);

        var input = cut.Find("input");
        input.KeyDown(Key.Down); // index 0
        input.KeyDown(Key.Down); // index 1
        input.KeyDown(Key.Down); // index 2
        input.KeyDown(Key.Down); // wraps to 0

        var items = cut.FindAll("li.here-autosuggest-item");
        Assert.That(items[0].ClassList, Does.Contain("active"));
    }

    [Test]
    public void Enter_Selects_Active_Item_And_Fires_Callback()
    {
        AutosuggestItem? selectedItem = null;
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.OnItemSelected, item => selectedItem = item));

        InjectResults(cut);

        var input = cut.Find("input");
        input.KeyDown(Key.Down); // select first item
        input.KeyDown(Key.Enter);

        Assert.That(selectedItem, Is.Not.Null);
        Assert.That(selectedItem!.Title, Is.EqualTo("Berlin"));
    }

    [Test]
    public void Escape_Closes_Dropdown()
    {
        var cut = Render<HereAutosuggest>();
        InjectResults(cut);

        Assert.That(cut.FindAll("ul.here-autosuggest-dropdown"), Has.Count.EqualTo(1));

        cut.Find("input").KeyDown(Key.Escape);

        Assert.That(cut.FindAll("ul.here-autosuggest-dropdown"), Has.Count.EqualTo(0));
    }

    [Test]
    public void Item_Click_Fires_OnItemSelected()
    {
        AutosuggestItem? selectedItem = null;
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.OnItemSelected, item => selectedItem = item));

        InjectResults(cut);

        cut.FindAll("li.here-autosuggest-item")[1].MouseDown();

        Assert.That(selectedItem, Is.Not.Null);
        Assert.That(selectedItem!.Title, Is.EqualTo("Bern"));
    }

    [Test]
    public void Custom_SuggestionItemTemplate_Renders_Custom_Markup()
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.SuggestionItemTemplate, item =>
                builder =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", "custom-item");
                    builder.AddContent(2, $"Custom: {item.Title}");
                    builder.CloseElement();
                }));

        InjectResults(cut);

        var customItems = cut.FindAll("span.custom-item");
        Assert.That(customItems, Has.Count.EqualTo(3));
        Assert.That(customItems[0].TextContent, Is.EqualTo("Custom: Berlin"));
    }

    [Test]
    public void Custom_SuggestionListTemplate_Renders_Custom_Markup()
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.SuggestionListTemplate, ctx =>
                builder =>
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "class", "custom-list");
                    foreach (var item in ctx.Items)
                    {
                        builder.OpenElement(2, "p");
                        builder.AddContent(3, item.Title);
                        builder.CloseElement();
                    }
                    builder.CloseElement();
                }));

        InjectResults(cut);

        var customList = cut.Find("div.custom-list");
        Assert.That(customList, Is.Not.Null);
        Assert.That(cut.FindAll("div.custom-list p"), Has.Count.EqualTo(3));
    }

    [Test]
    public void Enter_Without_Active_Item_Does_Nothing()
    {
        AutosuggestItem? selectedItem = null;
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.OnItemSelected, item => selectedItem = item));

        InjectResults(cut);

        // Press Enter without navigating — activeIndex is -1
        cut.Find("input").KeyDown(Key.Enter);

        Assert.That(selectedItem, Is.Null);
    }

    [Test]
    public void Selection_Closes_Dropdown_And_Updates_Value()
    {
        string? value = null;
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.ValueChanged, v => value = v));

        InjectResults(cut);

        var input = cut.Find("input");
        input.KeyDown(Key.Down);
        input.KeyDown(Key.Enter);

        // Dropdown should be closed
        Assert.That(cut.FindAll("ul.here-autosuggest-dropdown"), Has.Count.EqualTo(0));
        // Value should be updated via ValueChanged
        Assert.That(value, Is.EqualTo("Berlin, Germany"));
    }

    [Test]
    public async Task OnAutosuggestError_Fires_OnError_Callback()
    {
        string? errorMessage = null;
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.OnError, msg => { errorMessage = msg; }));

        await cut.InvokeAsync(() =>
            cut.Instance.OnAutosuggestError("HERE API authentication failed. Check your API key."));

        Assert.That(errorMessage, Is.EqualTo("HERE API authentication failed. Check your API key."));
    }

    [Test]
    public void Search_With_Default_Options_Sends_At_Parameter()
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.DebounceMs, 1));

        cut.Find("input").Input("Berlin");

        WaitUntil(() => GetAutosuggestInvocations().Count == 1);

        // The HERE Autosuggest API rejects in=countryCode without at (HTTP 400) —
        // the default options must produce a request that carries the at coordinate.
        var jsOptions = System.Text.Json.JsonSerializer.Serialize(GetAutosuggestInvocations()[0].Arguments[2]);
        Assert.That(jsOptions, Does.Contain("\"in\":\"countryCode:DEU\""));
        Assert.That(jsOptions, Does.Contain("\"lat\":51.1657"));
        Assert.That(jsOptions, Does.Contain("\"lng\":10.4515"));
        // Without show=details the API returns only address.label — the default
        // options must request the structured address fields.
        Assert.That(jsOptions, Does.Contain("\"show\":\"details\""));
    }

    [Test]
    public void Search_With_CircleFilter_Omits_At_Parameter()
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.DebounceMs, 1)
            .Add(x => x.Options, new AutosuggestOptions { In = "circle:52.5,13.4;r=10000" }));

        cut.Find("input").Input("Berlin");

        WaitUntil(() => GetAutosuggestInvocations().Count == 1);

        // at and in=circle/bbox are mutually exclusive per the HERE API.
        var jsOptions = System.Text.Json.JsonSerializer.Serialize(GetAutosuggestInvocations()[0].Arguments[2]);
        Assert.That(jsOptions, Does.Contain("circle:52.5,13.4;r=10000"));
        Assert.That(jsOptions, Does.Contain("\"at\":null"));
    }

    [Test]
    public void Search_With_CountryCodeOnly_Without_At_Fires_OnError_And_Skips_Request()
    {
        string? errorMessage = null;
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.DebounceMs, 1)
            .Add(x => x.Options, new AutosuggestOptions { In = "countryCode:DEU", At = null })
            .Add(x => x.OnError, msg => { errorMessage = msg; }));

        cut.Find("input").Input("Berlin");

        WaitUntil(() => errorMessage is not null);

        Assert.That(errorMessage, Does.Contain("spatial context"));
        Assert.That(GetAutosuggestInvocations(), Is.Empty);
    }

    [Test]
    public void Validation_Error_Clears_Previously_Open_Dropdown()
    {
        string? errorMessage = null;
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.DebounceMs, 1)
            .Add(x => x.Options, new AutosuggestOptions { In = "countryCode:DEU", At = null })
            .Add(x => x.OnError, msg => { errorMessage = msg; }));

        // Results from a previous (valid) search are still visible…
        InjectResults(cut);
        Assert.That(cut.FindAll("li.here-autosuggest-item"), Has.Count.EqualTo(3));

        // …then the next keystroke runs into the validation error.
        cut.Find("input").Input("Berlin");

        WaitUntil(() => errorMessage is not null);
        cut.WaitForAssertion(() =>
            Assert.That(cut.FindAll("ul.here-autosuggest-dropdown"), Has.Count.EqualTo(0)));
    }

    // The debounce in OnInput continues on a timer thread, so render-coupled
    // helpers like WaitForAssertion can miss the completion — poll instead.
    private static void WaitUntil(Func<bool> condition, int timeoutMs = 5000)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (!condition() && sw.ElapsedMilliseconds < timeoutMs)
            Thread.Sleep(10);
        Assert.That(condition(), Is.True, "Condition was not met within the timeout.");
    }

    private List<JSRuntimeInvocation> GetAutosuggestInvocations() =>
        JSInterop.Invocations
            .Where(i => i.Identifier == "herePlatform.objectManager.autosuggest")
            .ToList();
}
