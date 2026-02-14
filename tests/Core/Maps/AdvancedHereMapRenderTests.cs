using Bunit;
using NUnit.Framework;

namespace HerePlatformComponents.Tests.Maps;

public class AdvancedHereMapRenderTests : BunitTestBase
{
    [Test]
    public void Renders_HereMap_As_Child()
    {
        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.Id, "adv-map"));

        // AdvancedHereMap renders a HereMap which renders a <div>
        var div = cut.Find("div");
        Assert.That(div.GetAttribute("id"), Is.EqualTo("adv-map"));
    }

    [Test]
    public void UserAttributes_Are_Forwarded_To_Inner_HereMap_Div()
    {
        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.Id, "adv-map")
            .AddUnmatched("data-testid", "my-advanced-map")
            .AddUnmatched("aria-label", "Advanced map"));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("data-testid"), Is.EqualTo("my-advanced-map"));
        Assert.That(div.GetAttribute("aria-label"), Is.EqualTo("Advanced map"));
    }

    [Test]
    public void CssClass_Is_Forwarded_To_Inner_HereMap()
    {
        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.CssClass, "outer-class"));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("class"), Is.EqualTo("outer-class"));
    }

    [Test]
    public void Height_Is_Forwarded_To_Inner_HereMap()
    {
        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.Height, "200px"));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("style"), Is.EqualTo("height: 200px;"));
    }
}
