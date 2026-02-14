using AngleSharp.Dom;
using Bunit;
using HerePlatformComponents.Maps.Search;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace HerePlatformComponents.Tests.Search;

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
}
