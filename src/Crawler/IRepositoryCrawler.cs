using ModelRepoBrowser.Models;

namespace ModelRepoBrowser.Crawler;

/// <summary>
/// Defines methods for reading INTERLIS repository tree with required ilisite.xml (following IliSite09 Model),
/// optional ilimodels.xml (following IliRepository09 & IliRepository20 Models) and optional ilidata.xml (following DatasetIdx16 Model).
/// </summary>
public interface IRepositoryCrawler
{
    /// <summary>
    /// Parse the repository tree from the root repository following subsidiary-Sites links only.
    /// </summary>
    /// <param name="rootRepositoryUri">The <see cref="Uri"/> of the repository at the root of the model repository tree.</param>
    /// <returns>Dictionary containing all repositories found in tree. Repository host is used as key. Repositories contain all found information. Root repository contains full tree. </returns>
    Task<IDictionary<string, Repository>> CrawlModelRepositories(Uri rootRepositoryUri);
}
