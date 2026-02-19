namespace HerePlatform.Docs.Models;

public record ParamDoc(string Name, string Type, bool Required, string Description, string? Default = null);
