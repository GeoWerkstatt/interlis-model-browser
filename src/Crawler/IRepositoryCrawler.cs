using ModelRepoBrowser.Models;

namespace ModelRepoBrowser.Crawler
{
    public interface IRepositoryCrawler
    {
        Task<IDictionary<string, Repository>> CrawlModelRepositories(Uri rootRepositoryUri);
    }
}