using Bunit;
using HerePlatformComponents.Maps.Events;
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

    [Test]
    public void OnError_Parameter_DoesNotBreakRendering()
    {
        MapErrorEventArgs? received = null;
        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.Id, "err-map")
            .Add(x => x.OnError, args => { received = args; }));

        var div = cut.Find("div");
        Assert.That(div.GetAttribute("id"), Is.EqualTo("err-map"));
    }

    [Test]
    public async Task OnMapError_InvokesOnErrorCallback()
    {
        MapErrorEventArgs? received = null;
        var cut = Render<AdvancedHereMap>(p => p
            .Add(x => x.OnError, args => { received = args; }));

        var instance = cut.Instance;
        await instance.OnMapError(new MapErrorEventArgs
        {
            Source = "tile",
            Message = "Authentication failed (HTTP 401).",
            StatusCode = 401
        });

        Assert.That(received, Is.Not.Null);
        Assert.That(received!.Source, Is.EqualTo("tile"));
        Assert.That(received.StatusCode, Is.EqualTo(401));
    }
}
