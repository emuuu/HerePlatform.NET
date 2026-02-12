using HerePlatformComponents.Maps.Events;

namespace HerePlatformComponents.Tests.Events;

[TestFixture]
public class BaseLayerChangeEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new BaseLayerChangeEventArgs();

        Assert.That(args.Type, Is.Null);
    }

    [Test]
    public void Type_IsSettable()
    {
        var args = new BaseLayerChangeEventArgs { Type = "baselayerchange" };

        Assert.That(args.Type, Is.EqualTo("baselayerchange"));
    }
}

[TestFixture]
public class EngineStateChangeEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new EngineStateChangeEventArgs();

        Assert.That(args.State, Is.EqualTo(0));
        Assert.That(args.Type, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var args = new EngineStateChangeEventArgs
        {
            State = 2,
            Type = "enginestatechange"
        };

        Assert.That(args.State, Is.EqualTo(2));
        Assert.That(args.Type, Is.EqualTo("enginestatechange"));
    }
}

[TestFixture]
public class StateChangeEventArgsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var args = new StateChangeEventArgs();

        Assert.That(args.State, Is.Null);
        Assert.That(args.Type, Is.Null);
    }

    [Test]
    public void AllProperties_AreSettable()
    {
        var args = new StateChangeEventArgs
        {
            State = "open",
            Type = "statechange"
        };

        Assert.That(args.State, Is.EqualTo("open"));
        Assert.That(args.Type, Is.EqualTo("statechange"));
    }

    [TestCase("open")]
    [TestCase("closed")]
    public void State_AcceptsInfoBubbleStates(string state)
    {
        var args = new StateChangeEventArgs { State = state };

        Assert.That(args.State, Is.EqualTo(state));
    }
}
