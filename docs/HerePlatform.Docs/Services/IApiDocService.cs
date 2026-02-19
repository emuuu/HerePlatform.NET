using HerePlatform.Docs.Models;

namespace HerePlatform.Docs.Services;

public interface IApiDocService
{
    Task InitializeAsync();
    ApiTypeDoc? GetApiDoc(string typeName);
}
