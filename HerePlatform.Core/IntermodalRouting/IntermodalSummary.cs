namespace HerePlatform.Core.IntermodalRouting;

/// <summary>
/// Travel summary for an intermodal route section.
/// </summary>
public class IntermodalSummary
{
    /// <summary>
    /// Duration in seconds.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Length in meters.
    /// </summary>
    public int Length { get; set; }
}
