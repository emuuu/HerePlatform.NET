using System.Collections.Generic;

namespace HerePlatform.Core.IntermodalRouting;

/// <summary>
/// A single intermodal route consisting of multiple transport sections.
/// </summary>
public class IntermodalRoute
{
    /// <summary>
    /// Ordered sections of the route (e.g. walk → transit → walk).
    /// </summary>
    public List<IntermodalSection>? Sections { get; set; }
}
