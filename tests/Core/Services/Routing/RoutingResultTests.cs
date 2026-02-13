using HerePlatformComponents.Maps.Services.Routing;

namespace HerePlatformComponents.Tests.Services.Routing;

[TestFixture]
public class RoutingResultTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var result = new RoutingResult();

        Assert.That(result.Routes, Is.Null);
    }

    [Test]
    public void Route_WithSections()
    {
        var result = new RoutingResult
        {
            Routes = new List<Route>
            {
                new Route
                {
                    Sections = new List<RouteSection>
                    {
                        new RouteSection
                        {
                            Polyline = "BFoz5xJ67i1B1B7PzIhaxL7Y",
                            Summary = new RouteSummary
                            {
                                Duration = 3600,
                                Length = 50000,
                                BaseDuration = 3400
                            },
                            Transport = "car"
                        }
                    }
                }
            }
        };

        Assert.That(result.Routes, Has.Count.EqualTo(1));
        Assert.That(result.Routes[0].Sections, Has.Count.EqualTo(1));
        Assert.That(result.Routes[0].Sections![0].Summary!.Duration, Is.EqualTo(3600));
        Assert.That(result.Routes[0].Sections![0].Summary!.Length, Is.EqualTo(50000));
    }

    [Test]
    public void RouteSummary_DefaultValues()
    {
        var summary = new RouteSummary();

        Assert.That(summary.Duration, Is.EqualTo(0));
        Assert.That(summary.Length, Is.EqualTo(0));
        Assert.That(summary.BaseDuration, Is.Null);
    }
}
