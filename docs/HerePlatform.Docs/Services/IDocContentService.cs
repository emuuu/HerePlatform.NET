using HerePlatform.Docs.Models;

namespace HerePlatform.Docs.Services;

public interface IDocContentService
{
    Task InitializeAsync();
    List<NavSection> GetNavSections();
    List<SearchEntry> Search(string query);
    Task<DocArticle?> GetArticleAsync(string slug);
}
