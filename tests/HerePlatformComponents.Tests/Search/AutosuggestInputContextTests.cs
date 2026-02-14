using HerePlatformComponents.Maps.Search;

namespace HerePlatformComponents.Tests.Search;

[TestFixture]
public class AutosuggestInputContextTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var context = new AutosuggestInputContext();

        Assert.That(context.Value, Is.Null);
        Assert.That(context.Placeholder, Is.Null);
        Assert.That(context.Disabled, Is.False);
        Assert.That(context.InputAttributes, Is.Not.Null);
        Assert.That(context.InputAttributes, Is.Empty);
    }

    [Test]
    public void InitProperties_AreSetCorrectly()
    {
        var attrs = new Dictionary<string, object>
        {
            ["value"] = "test",
            ["autocomplete"] = "off"
        };

        var context = new AutosuggestInputContext
        {
            Value = "Berlin",
            Placeholder = "Enter an address...",
            Disabled = true,
            InputAttributes = attrs
        };

        Assert.That(context.Value, Is.EqualTo("Berlin"));
        Assert.That(context.Placeholder, Is.EqualTo("Enter an address..."));
        Assert.That(context.Disabled, Is.True);
        Assert.That(context.InputAttributes, Has.Count.EqualTo(2));
        Assert.That(context.InputAttributes["value"], Is.EqualTo("test"));
        Assert.That(context.InputAttributes["autocomplete"], Is.EqualTo("off"));
    }

    [Test]
    public void InputAttributes_DefaultDictionary_CanBePopulated()
    {
        var context = new AutosuggestInputContext();

        context.InputAttributes["placeholder"] = "Search...";
        context.InputAttributes["disabled"] = false;

        Assert.That(context.InputAttributes, Has.Count.EqualTo(2));
        Assert.That(context.InputAttributes["placeholder"], Is.EqualTo("Search..."));
    }
}
