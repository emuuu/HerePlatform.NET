using HerePlatformComponents.Maps.Services.MatrixRouting;

namespace HerePlatformComponents.Tests.Services.MatrixRouting;

[TestFixture]
public class MatrixRoutingResultTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var result = new MatrixRoutingResult();

        Assert.That(result.NumOrigins, Is.EqualTo(0));
        Assert.That(result.NumDestinations, Is.EqualTo(0));
        Assert.That(result.Matrix, Is.Empty);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var entries = new List<MatrixEntry>
        {
            new() { OriginIndex = 0, DestinationIndex = 0, Duration = 3600, Length = 50000 },
            new() { OriginIndex = 0, DestinationIndex = 1, Duration = 7200, Length = 100000 }
        };

        var result = new MatrixRoutingResult
        {
            NumOrigins = 1,
            NumDestinations = 2,
            Matrix = entries
        };

        Assert.That(result.NumOrigins, Is.EqualTo(1));
        Assert.That(result.NumDestinations, Is.EqualTo(2));
        Assert.That(result.Matrix, Has.Exactly(2).Items);
    }
}

[TestFixture]
public class MatrixEntryTests
{
    [Test]
    public void DefaultValues_AreZero()
    {
        var entry = new MatrixEntry();

        Assert.That(entry.OriginIndex, Is.EqualTo(0));
        Assert.That(entry.DestinationIndex, Is.EqualTo(0));
        Assert.That(entry.Duration, Is.EqualTo(0));
        Assert.That(entry.Length, Is.EqualTo(0));
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var entry = new MatrixEntry
        {
            OriginIndex = 2,
            DestinationIndex = 3,
            Duration = 1800,
            Length = 25000
        };

        Assert.That(entry.OriginIndex, Is.EqualTo(2));
        Assert.That(entry.DestinationIndex, Is.EqualTo(3));
        Assert.That(entry.Duration, Is.EqualTo(1800));
        Assert.That(entry.Length, Is.EqualTo(25000));
    }
}
