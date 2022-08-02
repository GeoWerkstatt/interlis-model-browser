using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Crawler;
using ModelRepoBrowser.Models;
using ModelRepoBrowser.TestHelpers;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text;

namespace ModelRepoBrowser;

[TestClass]
public class RepositoryCrawlerTest
{
    private Mock<ILogger<RepositoryCrawler>> loggerMock;
    private Mock<IHttpClientFactory> httpClientFactory;
    private IRepositoryCrawler repositoryCrawler;
    private MockHttpMessageHandler mockHttp;

    [TestInitialize]
    public void Initialize()
    {
        mockHttp = new MockHttpMessageHandler();
        SetupHttpMockFiles();

        SetupRepositoryCrawlerInstance(mockHttp.ToHttpClient());
    }

    private void SetupHttpMockFiles()
    {
        foreach (var dir in Directory.GetDirectories("./Testdata"))
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                mockHttp.When($"https://{Path.GetFileName(dir)}/{Path.GetFileName(file)}")
                    .Respond("application/xml", new FileStream(file, FileMode.Open, FileAccess.Read));

                mockHttp.When(HttpMethod.Head, $"https://{Path.GetFileName(dir)}/").Respond(HttpStatusCode.OK);
            }
        }
    }

    private void SetupRepositoryCrawlerInstance(HttpClient httpClient)
    {
        loggerMock = new Mock<ILogger<RepositoryCrawler>>();
        httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(cf => cf.CreateClient("")).Returns(httpClient);
        repositoryCrawler = new RepositoryCrawler(loggerMock.Object, httpClientFactory.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        httpClientFactory.VerifyAll();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public async Task CrawlerFindsAllRepositoriesInTree()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.ch"));
        Assert.IsNotNull(result);
        result.AssertAllNotNull();
        result
            .AssertSingleItem("models.interlis.ch", AssertModelsInterlisCh)
            .AssertSingleItem("models.geo.admin.ch", AssertModelsGeoAdminCh)
            .AssertCount(3);
    }

    [TestMethod]
    public async Task CrawlerProducesErrorLogsIfIlisiteXmlIsNotFound()
    {
        await repositoryCrawler.CrawlModelRepositories(new Uri("https://undefined.models.ch"));
        loggerMock.Verify(LogLevel.Error, "Analysis of https://undefined.models.ch/ failed.", Times.Once());
        loggerMock.Verify(LogLevel.Warning, "Could not analyse https://undefined.models.ch/ilidata.xml", Times.Once());
    }

    [TestMethod]
    public async Task CrawlerProducesWarningLogsIfIlidataXmlIsNotFound()
    {
        await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.ch"));
        loggerMock.Verify(LogLevel.Warning,  "Could not analyse https://models.interlis.ch/ilidata.xml.", Times.Once());
    }

    private void AssertModelsInterlisCh(Repository repository)
    {
        Assert.AreEqual("models.interlis.ch", repository.HostNameId);
        Assert.AreEqual(new Uri("https://models.interlis.ch/"), repository.Uri);
        Assert.AreEqual("interlis.ch", repository.Name);
        Assert.AreEqual("Allgemeine technische INTERLIS-Modelle", repository.Title);
        Assert.AreEqual("Modell-Ablage des INTERLIS-Kernteams", repository.ShortDescription);
        Assert.AreEqual("http://www.interlis.ch", repository.Owner);
        Assert.AreEqual("mailto:info@interlis.ch", repository.TechnicalContact);
        repository.SubsidiarySites
            .AssertSingleItem(ss => "models.geo.admin.ch".Equals(ss.Name, StringComparison.OrdinalIgnoreCase), AssertModelsGeoAdminCh)
            .AssertCount(2);
        repository.ParentSites.AssertCount(0);
        repository.Models.AssertCount(76).AssertAllNotNull();
        repository.Catalogs.AssertCount(0).AssertAllNotNull();
    }

    private void AssertModelsGeoAdminCh(Repository repository)
    {
        Assert.AreEqual("models.geo.admin.ch", repository.HostNameId);
        Assert.AreEqual(new Uri("https://models.geo.admin.ch/"), repository.Uri);
        Assert.AreEqual("models.geo.admin.ch", repository.Name);
        Assert.AreEqual("Geobasisdaten-Modell-Ablage", repository.Title);
        Assert.AreEqual("Modell-Ablage fuer Datenmodelle der Geobasisdaten des Bundesrechts", repository.ShortDescription);
        Assert.AreEqual("http://www.geo.admin.ch", repository.Owner);
        Assert.AreEqual("mailto:models@geo.admin.ch", repository.TechnicalContact);
        repository.SubsidiarySites.AssertCount(1);
        repository.ParentSites.AssertCount(1).AssertAllNotNull();
        repository.Models.AssertCount(1258).AssertAllNotNull();
        repository.Catalogs.AssertCount(170)
            .AssertSingleItem(
            c => "ch.admin.geo.models.bearbeitungsstatus_kataloge_20140701".Equals(c.Identifier, StringComparison.Ordinal),
            AssertBearbeitungstatusCatalog);
    }

    private void AssertBearbeitungstatusCatalog(Catalog catalog)
    {
        Assert.AreEqual("ch.admin.geo.models.bearbeitungsstatus_kataloge_20140701", catalog.Identifier);
        Assert.AreEqual("2014-07-01", catalog.Version);
        Assert.AreEqual(null, catalog.PrecursorVersion);
        Assert.AreEqual(new DateTime(2014, 7, 1, 0, 0, 0, DateTimeKind.Utc), catalog.PublishingDate);
        Assert.AreEqual("mailto:models@geo.admin.ch", catalog.Owner);
        Assert.AreEqual(string.Empty, catalog.Title);
        catalog.File.AssertContains("https://models.geo.admin.ch/BLW/bearbeitungsstatus_kataloge_20140701.xml").AssertCount(1);
        catalog.ReferencedModels.AssertContains("Biodiversitaetsfoerderflaechen_Qualitaetsstufe_II_und_Vernetzung_LV95_V1_3").AssertCount(22).AssertAllNotNull();
    }

    [TestMethod]
    public async Task CrawlerPerformsHttpsUpgrade()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Head, "https://models.interlis.ch").Respond(HttpStatusCode.OK);
        mockHttp.Expect("https://models.interlis.ch/ilisite.xml").Respond(HttpStatusCode.NotFound);
        mockHttp.Expect("https://models.interlis.ch/ilimodels.xml").Respond(HttpStatusCode.NotFound);
        mockHttp.Expect("https://models.interlis.ch/ilidata.xml").Respond(HttpStatusCode.NotFound);

        SetupRepositoryCrawlerInstance(mockHttp.ToHttpClient());
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("http://models.interlis.ch"));

        result.AssertCount(0);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task CrawlerUsesFallbackUrlIfHttpsFails()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Head, "https://models.interlis.ch").Respond(HttpStatusCode.HttpVersionNotSupported);
        mockHttp.Expect("http://models.interlis.ch/ilisite.xml").Respond(HttpStatusCode.NotFound);
        mockHttp.Expect("http://models.interlis.ch/ilimodels.xml").Respond(HttpStatusCode.NotFound);
        mockHttp.Expect("http://models.interlis.ch/ilidata.xml").Respond(HttpStatusCode.NotFound);

        SetupRepositoryCrawlerInstance(mockHttp.ToHttpClient());
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("http://models.interlis.ch"));

        result.AssertCount(0);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task CrawlerSkipsMultipleOccurences()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.ch"));
        result.AssertAllNotNull();
        result
            .AssertSingleItem("models.multiparent.ch", AssertModelsMultiparentCh)
            .AssertCount(3);
    }

    private void AssertModelsMultiparentCh(Repository repository)
    {
        Assert.AreEqual("models.multiparent.ch", repository.HostNameId);
        Assert.AreEqual(new Uri("https://models.multiparent.ch/"), repository.Uri);
        Assert.AreEqual("Minimal Multi Parent Repo", repository.Name);
        Assert.IsNull(repository.Title);
        Assert.IsNull(repository.ShortDescription);
        Assert.IsNull(repository.Owner);
        Assert.IsNull(repository.TechnicalContact);
        repository.SubsidiarySites.AssertCount(0);
        repository.ParentSites
            .AssertSingleItem(ps => "models.interlis.ch".Equals(ps.HostNameId, StringComparison.OrdinalIgnoreCase), AssertModelsInterlisCh)
            .AssertSingleItem(ps => "models.geo.admin.ch".Equals(ps.HostNameId, StringComparison.OrdinalIgnoreCase), AssertModelsGeoAdminCh)
            .AssertCount(2);
        repository.Models.AssertCount(0).AssertAllNotNull();
        repository.Catalogs.AssertCount(0).AssertAllNotNull();
    }

    [TestMethod]
    public async Task CrawlerDoesSupportBadFormattedXml()
    {
        mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When($"https://models.interlis.ch/ilidata.xml")
            .Respond("application/xml", new MemoryStream(Encoding.UTF8.GetBytes("Bad Formatted xml stream")));
        mockHttp
            .When($"https://models.geo.admin.ch/ilimodels.xml")
            .Respond("application/xml", new MemoryStream(Encoding.UTF8.GetBytes("Bad Formatted xml stream")));
        mockHttp
            .When($"https://models.multiparent.ch/ilisite.xml")
            .Respond("application/xml", new MemoryStream(Encoding.UTF8.GetBytes("Bad Formatted xml stream")));

        SetupHttpMockFiles();
        SetupRepositoryCrawlerInstance(mockHttp.ToHttpClient());

        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.ch"));
        result.AssertAllNotNull();
        result.AssertCount(2);
    }
}
