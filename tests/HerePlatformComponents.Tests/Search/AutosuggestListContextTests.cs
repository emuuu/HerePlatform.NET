using HerePlatform.Core.Coordinates;
using HerePlatformComponents.Maps;
using HerePlatformComponents.Maps.Search;

namespace HerePlatformComponents.Tests.Search;

[TestFixture]
public class AutosuggestListContextTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var context = new AutosuggestListContext();

        Assert.That(context.Items, Is.Not.Null);
        Assert.That(context.Items, Is.Empty);
        Assert.That(context.ActiveIndex, Is.EqualTo(-1));
        Assert.That(context.SelectItem, Is.Not.Null);
    }

    [Test]
    public async Task DefaultSelectItem_DoesNotThrow()
    {
        var context = new AutosuggestListContext();
        var item = new AutosuggestItem { Title = "Test" };

        Assert.DoesNotThrowAsync(() => context.SelectItem(item));
    }

    [Test]
    public void Items_CanBeInitialized()
    {
        var items = new List<AutosuggestItem>
        {
            new() { Title = "Brandenburger Tor", ResultType = "place" },
            new() { Title = "Alexanderplatz", ResultType = "street" },
            new() { Title = "Potsdamer Platz", ResultType = "place" }
        };

        var context = new AutosuggestListContext
        {
            Items = items.AsReadOnly(),
            ActiveIndex = 1
        };

        Assert.That(context.Items, Has.Count.EqualTo(3));
        Assert.That(context.Items[0].Title, Is.EqualTo("Brandenburger Tor"));
        Assert.That(context.Items[1].Title, Is.EqualTo("Alexanderplatz"));
        Assert.That(context.Items[2].Title, Is.EqualTo("Potsdamer Platz"));
        Assert.That(context.ActiveIndex, Is.EqualTo(1));
    }

    [Test]
    public async Task SelectItem_InvokesCallback()
    {
        AutosuggestItem? selectedItem = null;

        var context = new AutosuggestListContext
        {
            SelectItem = item =>
            {
                selectedItem = item;
                return Task.CompletedTask;
            }
        };

        var testItem = new AutosuggestItem
        {
            Title = "Brandenburger Tor",
            Position = new LatLngLiteral(52.5163, 13.3777)
        };

        await context.SelectItem(testItem);

        Assert.That(selectedItem, Is.Not.Null);
        Assert.That(selectedItem!.Title, Is.EqualTo("Brandenburger Tor"));
        Assert.That(selectedItem.Position!.Value.Lat, Is.EqualTo(52.5163));
    }

    [Test]
    public void ActiveIndex_NegativeOne_MeansNoSelection()
    {
        var context = new AutosuggestListContext
        {
            Items = new List<AutosuggestItem>
            {
                new() { Title = "Item 1" },
                new() { Title = "Item 2" }
            }.AsReadOnly(),
            ActiveIndex = -1
        };

        Assert.That(context.ActiveIndex, Is.LessThan(0));
    }

    [Test]
    public void Items_AsReadOnlyList_PreventsDirectModification()
    {
        var items = new List<AutosuggestItem>
        {
            new() { Title = "Item 1" }
        };

        var context = new AutosuggestListContext
        {
            Items = items.AsReadOnly()
        };

        Assert.That(context.Items, Is.InstanceOf<IReadOnlyList<AutosuggestItem>>());
        Assert.That(context.Items, Has.Count.EqualTo(1));
    }
}
