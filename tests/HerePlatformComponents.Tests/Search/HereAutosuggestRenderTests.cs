using Bunit;
using HerePlatformComponents.Maps.Search;
using NUnit.Framework;

namespace HerePlatformComponents.Tests.Search;

public class HereAutosuggestRenderTests : BunitTestBase
{
    [Test]
    public void Renders_Wrapper_Div_With_Autosuggest_Class()
    {
        var cut = Render<HereAutosuggest>();

        var div = cut.Find("div.here-autosuggest");
        Assert.That(div, Is.Not.Null);
    }

    [Test]
    public void Renders_Input_With_Placeholder()
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.Placeholder, "Search here..."));

        var input = cut.Find("input");
        Assert.That(input.GetAttribute("placeholder"), Is.EqualTo("Search here..."));
    }

    [Test]
    [TestCase(AutosuggestDesign.Default, "here-autosuggest-default")]
    [TestCase(AutosuggestDesign.Compact, "here-autosuggest-compact")]
    [TestCase(AutosuggestDesign.Filled, "here-autosuggest-filled")]
    [TestCase(AutosuggestDesign.Outlined, "here-autosuggest-outlined")]
    [TestCase(AutosuggestDesign.Rounded, "here-autosuggest-rounded")]
    public void Design_Variant_Sets_Correct_Css_Class(AutosuggestDesign design, string expectedClass)
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.Design, design));

        var div = cut.Find("div.here-autosuggest");
        Assert.That(div.ClassList, Does.Contain(expectedClass));
    }

    [Test]
    public void Disabled_Sets_Disabled_On_Input()
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.Disabled, true));

        var input = cut.Find("input");
        Assert.That(input.HasAttribute("disabled"), Is.True);
    }

    [Test]
    public void UserAttributes_Are_Rendered_On_Outer_Div()
    {
        var cut = Render<HereAutosuggest>(p => p
            .AddUnmatched("data-testid", "search-box")
            .AddUnmatched("aria-label", "Search"));

        var div = cut.Find("div.here-autosuggest");
        Assert.That(div.GetAttribute("data-testid"), Is.EqualTo("search-box"));
        Assert.That(div.GetAttribute("aria-label"), Is.EqualTo("Search"));
    }

    [Test]
    public void Dropdown_Not_Visible_Initially()
    {
        var cut = Render<HereAutosuggest>();

        Assert.That(cut.FindAll("ul.here-autosuggest-dropdown"), Has.Count.EqualTo(0));
    }

    [Test]
    public void CssClass_Is_Applied_To_Wrapper()
    {
        var cut = Render<HereAutosuggest>(p => p
            .Add(x => x.CssClass, "my-custom"));

        var div = cut.Find("div.here-autosuggest");
        Assert.That(div.ClassList, Does.Contain("my-custom"));
    }
}
