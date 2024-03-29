﻿using ModelRepoBrowser.Crawler.XmlModels;
using ModelRepoBrowser.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace ModelRepoBrowser.Crawler;

/// <inheritdoc />
public class RepositoryCrawler : IRepositoryCrawler
{
    private readonly ILogger<RepositoryCrawler> logger;
    private readonly HttpClient httpClient;
    private ConcurrentDictionary<string, Repository> modelRepositories;

    public RepositoryCrawler(ILogger<RepositoryCrawler> logger, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        httpClient = httpClientFactory.CreateClient();
    }

    /// <inheritdoc />
    public async Task<IDictionary<string, Repository>> CrawlModelRepositories(Uri rootRepositoryUri)
    {
        modelRepositories = new ConcurrentDictionary<string, Repository>();
        await CrawlRepositories(rootRepositoryUri, null).ConfigureAwait(false);
        return modelRepositories;
    }

    private async Task<Repository?> CrawlRepositories(Uri rootRepositoryUri, Repository? parent)
    {
        rootRepositoryUri = await PreferHttpsIfAvailable(rootRepositoryUri).ConfigureAwait(false);

        if (modelRepositories.TryGetValue(rootRepositoryUri.AbsoluteUri, out var existingRepository))
        {
            if (parent != null)
            {
                existingRepository.ParentSites.Add(parent);
            }

            return existingRepository;
        }

        var (root, subsidiarys) = await AnalyseRepository(rootRepositoryUri, parent).ConfigureAwait(false);
        if (root is not null && modelRepositories.TryAdd(rootRepositoryUri.AbsoluteUri, root))
        {
            var subsidiarySites = new HashSet<Repository>();
            foreach (var subsidiary in subsidiarys)
            {
                var subsidiaryRepository = await CrawlRepositories(subsidiary, root).ConfigureAwait(false);
                if (subsidiaryRepository != null)
                {
                    subsidiarySites.Add(subsidiaryRepository);
                }
            }

            root.SubsidiarySites = subsidiarySites;
        }

        return root;
    }

    private async Task<(Repository? Repository, IEnumerable<Uri> SubsidiaryRepositories)> AnalyseRepository(Uri repositoryUri, Repository? parent = null)
    {
        try
        {
            var ilisiteTask = ParseIlisite(repositoryUri);
            var modelsTask = CrawlIlimodels(repositoryUri);
            var metadatasTask = CrawlIlidata(repositoryUri);

            var ilisite = await ilisiteTask.ConfigureAwait(false);
            var models = await modelsTask.ConfigureAwait(false);
            var metadatas = await metadatasTask.ConfigureAwait(false);

            if (ilisite is null) return (null, Enumerable.Empty<Uri>());

            var repository = new Repository
            {
                HostNameId = repositoryUri.AbsoluteUri,
                Uri = repositoryUri,
                Name = ilisite.Name ?? repositoryUri.Host,
                Title = ilisite.Title,
                ShortDescription = ilisite.shortDescription,
                Owner = ilisite.Owner,
                TechnicalContact = ilisite.technicalContact,
                Models = models,
                Catalogs = metadatas,
            };

            if (parent != null)
            {
                repository.ParentSites.Add(parent);
            }

            foreach (var model in models)
            {
                model.ModelRepository = repository;
            }

            var subsidiaryRepositories = ilisite.subsidiarySites?
                    .Where(location => location?.value is not null)
                    .Select(location => new Uri(location.value!))
                    .ToList() ?? Enumerable.Empty<Uri>();

            return (repository, subsidiaryRepositories);
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is InvalidOperationException)
        {
            logger.LogError(ex, "Analysis of {Repository} failed.", repositoryUri);
            return (null, Enumerable.Empty<Uri>());
        }
    }

    private async Task<ISet<Catalog>> CrawlIlidata(Uri repositoryUri)
    {
        var ilidataUri = GetIlidataUrl(repositoryUri);
        try
        {
            using (var ilidataStream = await GetStreamFromUrl(ilidataUri).ConfigureAwait(false))
            {
                return RepositoryFilesDeserializer.ParseIliData(ilidataStream)
                    .Select(m => new Catalog
                    {
                        Identifier = m.id,
                        Version = m.version,
                        PublishingDate = DateTime.SpecifyKind(m.publishingDate.Date, DateTimeKind.Utc),
                        PrecursorVersion = m.precursorVersion,
                        Owner = m.owner,
                        Title = m.GetTitle(),
                        File = m.GetFiles().Select(f => repositoryUri.Append(f).AbsoluteUri).ToList(),
                        ReferencedModels = m.GetReferencedModels(),
                    })
                    .RemovePrecursorCatalogVersions()
                    .ToHashSet();
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Could not analyse {IliDataUri}.", ilidataUri);
        }

        return new HashSet<Catalog>();
    }

    private async Task<ISet<Model>> CrawlIlimodels(Uri repositoryUri)
    {
        var ilimodelsUri = GetIlimodelsUrl(repositoryUri);
        using (var ilimodelsStream = await GetStreamFromUrl(ilimodelsUri).ConfigureAwait(false))
        {
            var models = RepositoryFilesDeserializer.ParseIliModels(ilimodelsStream)
                .Select(model => new Model
                {
                    Name = model.Name,
                    SchemaLanguage = model.SchemaLanguage,
                    File = model.File,
                    Version = model.Version,
                    PublishingDate = model.publishingDate?.ToUniversalTime(),
                    DependsOnModel = model.dependsOnModel.Where(s => !string.IsNullOrEmpty(s?.value)).Select(m => m.value!).ToList(),
                    ShortDescription = model.shortDescription,
                    Title = model.Title,
                    Issuer = model.Issuer,
                    TechnicalContact = model.technicalContact,
                    FurtherInformation = model.furtherInformation,
                    MD5 = model.md5,
                    Tags = model.Tags?.Split(',').Distinct().ToList() ?? new List<string>(),
                })
                .ToHashSet();

            foreach (var model in models)
            {
                if (string.IsNullOrEmpty(model.MD5))
                {
                    var modelFileUrl = repositoryUri.Append(model.File);
                    logger.LogInformation("Calculate missing MD5 for Model <{Model}> in File <{URL}>.", model.Name, modelFileUrl);

                    try
                    {
                        var stream = await GetStreamFromUrl(modelFileUrl).ConfigureAwait(false);
                        var md5 = await GetMD5FromStream(stream).ConfigureAwait(false);
                        model.MD5 = md5;
                    }
                    catch (HttpRequestException ex)
                    {
                        logger.LogError(ex, "Failed to calculate missing MD5 for Model <{Model}> in File <{URL}>", model.Name, modelFileUrl);
                    }
                }
            }

            return models;
        }
    }

    private async Task<Site?> ParseIlisite(Uri repositoryUri)
    {
        var ilisiteUri = GetIlisiteUrl(repositoryUri);
        using (var ilisiteStream = await GetStreamFromUrl(ilisiteUri).ConfigureAwait(false))
        {
            return RepositoryFilesDeserializer.ParseIliSite(ilisiteStream);
        }
    }

    private async Task<Stream> GetStreamFromUrl(Uri url)
    {
        var response = await httpClient.GetAsync(url).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var content = response.Content;
        return await content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    internal async Task<string> GetMD5FromStream(Stream stream)
    {
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
        using var md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
        var hash = await md5.ComputeHashAsync(stream).ConfigureAwait(false);
        return Convert.ToHexString(hash);
    }

    private async Task<Uri> PreferHttpsIfAvailable(Uri uri)
    {
        if (Uri.UriSchemeHttps.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            return uri;

        try
        {
            var httpsUri = new Uri(uri.OriginalString.Replace(Uri.UriSchemeHttp, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
            using (var request = new HttpRequestMessage(HttpMethod.Head, httpsUri))
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                return response.IsSuccessStatusCode ? httpsUri : uri;
            }
        }
        catch (HttpRequestException)
        {
            return uri;
        }
    }

    private static Uri GetIlisiteUrl(Uri baseUri) => baseUri.Append("/ilisite.xml");

    private static Uri GetIlimodelsUrl(Uri baseUri) => baseUri.Append("/ilimodels.xml");

    private static Uri GetIlidataUrl(Uri baseUri) => baseUri.Append("/ilidata.xml");
}
