using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Services.Isoline;

namespace HerePlatformComponents.Tests.Services.Isoline;

[TestFixture]
public class IsolineResultTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var result = new IsolineResult();

        Assert.That(result.Isolines, Is.Null);
    }

    [Test]
    public void WithIsolines()
    {
        var result = new IsolineResult
        {
            Isolines = new List<IsolinePolygon>
            {
                new IsolinePolygon
                {
                    Range = 600,
                    Polygon = new List<LatLngLiteral>
                    {
                        new(52.53, 13.38),
                        new(52.53, 13.43),
                        new(52.51, 13.43),
                        new(52.51, 13.38)
                    }
                }
            }
        };

        Assert.That(result.Isolines, Has.Count.EqualTo(1));
        Assert.That(result.Isolines[0].Range, Is.EqualTo(600));
        Assert.That(result.Isolines[0].Polygon, Has.Count.EqualTo(4));
    }

    [Test]
    public void IsolinePolygon_DefaultValues()
    {
        var isoline = new IsolinePolygon();

        Assert.That(isoline.Range, Is.EqualTo(0));
        Assert.That(isoline.Polygon, Is.Null);
        Assert.That(isoline.EncodedPolyline, Is.Null);
    }
}
