using HerePlatform.Core.Coordinates;
using HerePlatform.Core.Routing;
using HerePlatformComponents.Maps;

namespace HerePlatformComponents.Tests.Services.Routing;

[TestFixture]
public class TurnInstructionTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var instruction = new TurnInstruction();

        Assert.That(instruction.Action, Is.Null);
        Assert.That(instruction.Instruction, Is.Null);
        Assert.That(instruction.Duration, Is.EqualTo(0));
        Assert.That(instruction.Length, Is.EqualTo(0));
        Assert.That(instruction.Offset, Is.EqualTo(0));
        Assert.That(instruction.Position, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var instruction = new TurnInstruction
        {
            Action = "turnLeft",
            Instruction = "Turn left onto Friedrichstrasse",
            Duration = 30,
            Length = 200,
            Offset = 15,
            Position = new LatLngLiteral(52.52, 13.405)
        };

        Assert.That(instruction.Action, Is.EqualTo("turnLeft"));
        Assert.That(instruction.Instruction, Is.EqualTo("Turn left onto Friedrichstrasse"));
        Assert.That(instruction.Duration, Is.EqualTo(30));
        Assert.That(instruction.Length, Is.EqualTo(200));
        Assert.That(instruction.Offset, Is.EqualTo(15));
        Assert.That(instruction.Position!.Value.Lat, Is.EqualTo(52.52));
    }

    [Test]
    public void RouteSection_TurnByTurnActions()
    {
        var section = new RouteSection
        {
            Summary = new RouteSummary { Duration = 3600, Length = 50000 },
            TurnByTurnActions = new List<TurnInstruction>
            {
                new TurnInstruction { Action = "depart", Instruction = "Head north", Duration = 10, Length = 50 },
                new TurnInstruction { Action = "turnRight", Instruction = "Turn right", Duration = 20, Length = 300 },
                new TurnInstruction { Action = "arrive", Instruction = "Arrive at destination", Duration = 0, Length = 0 }
            }
        };

        Assert.That(section.TurnByTurnActions, Has.Count.EqualTo(3));
        Assert.That(section.TurnByTurnActions[0].Action, Is.EqualTo("depart"));
        Assert.That(section.TurnByTurnActions[2].Action, Is.EqualTo("arrive"));
    }

    [Test]
    public void RoutingRequest_ReturnInstructions_Default()
    {
        var request = new RoutingRequest();
        Assert.That(request.ReturnInstructions, Is.False);
    }

    [Test]
    public void RoutingRequest_ReturnInstructions_Settable()
    {
        var request = new RoutingRequest { ReturnInstructions = true };
        Assert.That(request.ReturnInstructions, Is.True);
    }
}
