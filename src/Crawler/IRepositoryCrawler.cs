using ModelRepoBrowser.Models;

namespace ModelRepoBrowser.Crawler;

public interface IRepositoryCrawler
{
    /// <summary>
    /// Parse the repository tree from the root repository following subsidiary links.
    /// </summary>
    /// <param name="rootRepositoryUri">The <see cref="Uri"/> of the repository at the root of the model repository tree.</param>
    /// <returns>Dictionary containing all repositories found in tree. Repository host is used as key. Repositories contain all found information. </returns>
    Task<IDictionary<string, Repository>> CrawlModelRepositories(Uri rootRepositoryUri);
}
