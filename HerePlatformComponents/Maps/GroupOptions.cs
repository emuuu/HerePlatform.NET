using System.Collections.Generic;

namespace HerePlatformComponents.Maps;

public class GroupOptions : ListableEntityOptionsBase
{
    /// <summary>
    /// Initial objects to add to the group.
    /// </summary>
    public IEnumerable<IJsObjectRef>? Objects { get; set; }
}
