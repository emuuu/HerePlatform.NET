using Bunit;
using NUnit.Framework;

namespace HerePlatformComponents.Tests.Maps;

public class HereMapRenderTests : BunitTestBase
{
    [Test]
    public void Renders_Div_With_Correct_Id()
    {
        var cut = Render<HereMap>(p => p
            .Add(x => x.Id, "my-map"));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("id"), Is.EqualTo("my-map"));
    }

    [Test]
    public void CssClass_Sets_Class_Attribute()
    {
        var cut = Render<HereMap>(p => p
            .Add(x => x.CssClass, "custom-class"));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("class"), Is.EqualTo("custom-class"));
    }

    [Test]
    public void Height_Sets_Style_Attribute()
    {
        var cut = Render<HereMap>(p => p
            .Add(x => x.Height, "300px"));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("style"), Is.EqualTo("height: 300px;"));
    }

    [Test]
    public void Default_Height_Is_500px()
    {
        var cut = Render<HereMap>();

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("style"), Is.EqualTo("height: 500px;"));
    }

    [Test]
    public void UserAttributes_Are_Rendered_On_Div()
    {
        var cut = Render<HereMap>(p => p
            .Add(x => x.Id, "test-map")
            .AddUnmatched("data-testid", "my-map")
            .AddUnmatched("aria-label", "Map view"));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("data-testid"), Is.EqualTo("my-map"));
        Assert.That(div.GetAttribute("aria-label"), Is.EqualTo("Map view"));
    }
}
