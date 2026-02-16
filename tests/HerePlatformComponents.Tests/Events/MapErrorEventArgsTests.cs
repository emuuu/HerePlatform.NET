using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class MapErrorEventArgsTests
{
    [Test]
    public void DefaultValues_AreNull()
    {
        var args = new MapErrorEventArgs();

        Assert.That(args.Source, Is.Null);
        Assert.That(args.Message, Is.Null);
        Assert.That(args.StatusCode, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var args = new MapErrorEventArgs
        {
            Source = "tile",
            Message = "Authentication failed (HTTP 401). Check your HERE API key.",
            StatusCode = 401
        };

        Assert.That(args.Source, Is.EqualTo("tile"));
        Assert.That(args.Message, Does.Contain("401"));
        Assert.That(args.StatusCode, Is.EqualTo(401));
    }
}
