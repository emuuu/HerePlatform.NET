using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class PolygonComponentOptionsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var component = new PolygonComponent();

        Assert.That(component.Path, Is.Null);
        Assert.That(component.StrokeColor, Is.Null);
        Assert.That(component.FillColor, Is.Null);
        Assert.That(component.LineWidth, Is.Null);
        Assert.That(component.LineCap, Is.Null);
        Assert.That(component.LineJoin, Is.Null);
        Assert.That(component.LineDash, Is.Null);
        Assert.That(component.LineDashOffset, Is.Null);
        Assert.That(component.ZIndex, Is.Null);
        Assert.That(component.Clickable, Is.False);
        Assert.That(component.Visible, Is.True);
        Assert.That(component.Data, Is.Null);
        Assert.That(component.Holes, Is.Null);
        Assert.That(component.Draggable, Is.False);
        Assert.That(component.Extrusion, Is.Null);
        Assert.That(component.Elevation, Is.Null);
    }

    [Test]
    public void Guid_IsUnique()
    {
        var a = new PolygonComponent();
        var b = new PolygonComponent();

        Assert.That(a.Guid, Is.Not.EqualTo(b.Guid));
    }

    [Test]
    public void Path_AcceptsList()
    {
        var component = new PolygonComponent();
        var path = new List<LatLngLiteral>
        {
            new(52.52, 13.39),
            new(52.52, 13.42),
            new(52.51, 13.42),
            new(52.51, 13.39)
        };

        component.Path = path;

        Assert.That(component.Path, Has.Exactly(4).Items);
    }

    [Test]
    public void Holes_AcceptsList()
    {
        var component = new PolygonComponent();
        var holes = new List<List<LatLngLiteral>>
        {
            new()
            {
                new(52.518, 13.400),
                new(52.518, 13.410),
                new(52.515, 13.410),
                new(52.515, 13.400)
            }
        };

        component.Holes = holes;

        Assert.That(component.Holes, Has.Exactly(1).Items);
        Assert.That(component.Holes[0], Has.Exactly(4).Items);
    }

    [Test]
    public async Task HandleGeometryChanged_UpdatesPath()
    {
        var component = new PolygonComponent();
        var newPath = new List<LatLngLiteral>
        {
            new(52.52, 13.39),
            new(52.53, 13.40),
            new(52.51, 13.41)
        };

        await component.HandleGeometryChanged(newPath);

        Assert.That(component.Path, Is.SameAs(newPath));
        Assert.That(component.Path, Has.Exactly(3).Items);
    }

    [Test]
    public void Serialization_OptionsValues()
    {
        var json = Helper.SerializeObject(new
        {
            path = new List<LatLngLiteral> { new(52.52, 13.39) },
            strokeColor = "#009900",
            fillColor = "rgba(0, 153, 0, 0.3)",
            lineWidth = 2.0,
            clickable = true,
            visible = true
        });

        Assert.That(json, Does.Contain("\"strokeColor\":\"#009900\""));
        Assert.That(json, Does.Contain("\"fillColor\":\"rgba(0, 153, 0, 0.3)\""));
        Assert.That(json, Does.Contain("\"lineWidth\":2"));
    }
}
