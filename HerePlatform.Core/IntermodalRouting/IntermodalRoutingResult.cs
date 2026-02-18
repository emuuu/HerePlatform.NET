using System.Collections.Generic;

namespace HerePlatform.Core.IntermodalRouting;

/// <summary>
/// Result of an Intermodal Routing API request.
/// </summary>
public class IntermodalRoutingResult
{
    /// <summary>
    /// Calculated intermodal routes.
    /// </summary>
    public List<IntermodalRoute>? Routes { get; set; }
}
