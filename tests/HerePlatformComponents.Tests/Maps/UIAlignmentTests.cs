using HerePlatformComponents.Maps.UI;

namespace HerePlatformComponents.Tests.Maps;

[TestFixture]
public class UIAlignmentTests
{
    [Test]
    public void UIAlignment_HasAllPositions()
    {
        var values = Enum.GetValues<UIAlignment>();

        Assert.That(values.Length, Is.EqualTo(12));
    }

    [Test]
    public void UIAlignment_TopLeft_IsFirst()
    {
        Assert.That((int)UIAlignment.TopLeft, Is.EqualTo(0));
    }

    [Test]
    public void UIAlignment_BottomRight_IsLast()
    {
        Assert.That((int)UIAlignment.BottomRight, Is.EqualTo(11));
    }

    [Test]
    public void UIAlignment_EnumMemberValues()
    {
        Assert.That(UIAlignment.TopLeft.ToString(), Is.EqualTo("TopLeft"));
        Assert.That(UIAlignment.BottomRight.ToString(), Is.EqualTo("BottomRight"));
    }
}
