using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class StyleOptionsTests
{
    [Test]
    public void DefaultValues_AreAllNull()
    {
        var style = new StyleOptions();

        Assert.That(style.StrokeColor, Is.Null);
        Assert.That(style.FillColor, Is.Null);
        Assert.That(style.LineWidth, Is.Null);
        Assert.That(style.LineCap, Is.Null);
        Assert.That(style.LineDash, Is.Null);
        Assert.That(style.LineDashImage, Is.Null);
        Assert.That(style.LineDashScaleMode, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var style = new StyleOptions
        {
            StrokeColor = "rgba(0, 0, 255, 0.5)",
            FillColor = "#FF0000",
            LineWidth = 3.5,
            LineCap = "round",
            LineDash = [10, 5, 2, 5],
            LineDashImage = "dash-pattern.png",
            LineDashScaleMode = "CONTINUOUS"
        };

        Assert.That(style.StrokeColor, Is.EqualTo("rgba(0, 0, 255, 0.5)"));
        Assert.That(style.FillColor, Is.EqualTo("#FF0000"));
        Assert.That(style.LineWidth, Is.EqualTo(3.5));
        Assert.That(style.LineCap, Is.EqualTo("round"));
        Assert.That(style.LineDash, Is.EqualTo(new double[] { 10, 5, 2, 5 }));
        Assert.That(style.LineDashImage, Is.EqualTo("dash-pattern.png"));
        Assert.That(style.LineDashScaleMode, Is.EqualTo("CONTINUOUS"));
    }
}
