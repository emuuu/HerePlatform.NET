using HerePlatformComponents.Maps.Search;

namespace HerePlatformComponents.Tests.Search;

[TestFixture]
public class AutosuggestDesignTests
{
    [Test]
    public void Enum_HasExpectedValues()
    {
        var values = Enum.GetValues<AutosuggestDesign>();

        Assert.That(values, Has.Length.EqualTo(5));
        Assert.That(values, Does.Contain(AutosuggestDesign.Default));
        Assert.That(values, Does.Contain(AutosuggestDesign.Compact));
        Assert.That(values, Does.Contain(AutosuggestDesign.Filled));
        Assert.That(values, Does.Contain(AutosuggestDesign.Outlined));
        Assert.That(values, Does.Contain(AutosuggestDesign.Rounded));
    }

    [TestCase(AutosuggestDesign.Default, 0)]
    [TestCase(AutosuggestDesign.Compact, 1)]
    [TestCase(AutosuggestDesign.Filled, 2)]
    [TestCase(AutosuggestDesign.Outlined, 3)]
    [TestCase(AutosuggestDesign.Rounded, 4)]
    public void Enum_HasExpectedIntegerValues(AutosuggestDesign design, int expected)
    {
        Assert.That((int)design, Is.EqualTo(expected));
    }

    [Test]
    public void Serialize_ProducesCamelCaseString()
    {
        var json = Helper.SerializeObject(new { design = AutosuggestDesign.Compact });

        Assert.That(json, Does.Contain("\"design\":\"compact\""));
    }

    [TestCase(AutosuggestDesign.Default, "default")]
    [TestCase(AutosuggestDesign.Compact, "compact")]
    [TestCase(AutosuggestDesign.Filled, "filled")]
    [TestCase(AutosuggestDesign.Outlined, "outlined")]
    [TestCase(AutosuggestDesign.Rounded, "rounded")]
    public void Serialize_EachVariant_ProducesExpectedValue(AutosuggestDesign design, string expected)
    {
        var json = Helper.SerializeObject(new { design });

        Assert.That(json, Does.Contain($"\"design\":\"{expected}\""));
    }
}
