﻿using Microsoft.Extensions.Logging;
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
    private RepositoryCrawler repositoryCrawler;
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
                mockHttp
                    .When($"https://{Path.GetFileName(dir)}/{Path.GetFileName(file)}")
                    .Respond("application/xml", new FileStream(file, FileMode.Open, FileAccess.Read));

                mockHttp
                    .When(HttpMethod.Head, $"https://{Path.GetFileName(dir)}/")
                    .Respond(HttpStatusCode.OK);
            }
        }

        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);
    }

    private void SetupRepositoryCrawlerInstance(HttpClient httpClient)
    {
        loggerMock = new Mock<ILogger<RepositoryCrawler>>();
        httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory
            .Setup(cf => cf.CreateClient(""))
            .Returns(httpClient);
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
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.testdata"));
        Assert.IsNotNull(result);
        result
            .AssertSingleItem("https://models.interlis.testdata/", AssertModelsInterlisCh)
            .AssertSingleItem("https://models.geo.admin.testdata/", AssertModelsGeoAdminCh)
            .AssertAllNotNull()
            .AssertCount(3);
    }

    [TestMethod]
    public async Task CrawlerProducesErrorLogsIfIlisiteXmlIsNotFound()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://undefined.models.testdata"));
        Assert.IsNotNull(result);
        result.AssertCount(0);
        loggerMock.Verify(LogLevel.Error, "Analysis of https://undefined.models.testdata/ failed.", Times.Once());
    }

    [TestMethod]
    public async Task CrawlerProducesWarningLogsIfIlidataXmlIsNotFound()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.testdata"));
        Assert.IsNotNull(result);
        result.Keys
            .AssertContains("https://models.interlis.testdata/")
            .AssertContains("https://models.multiparent.testdata/")
            .AssertContains("https://models.geo.admin.testdata/")
            .AssertCount(3);
        loggerMock.Verify(LogLevel.Warning,  "Could not analyse https://models.interlis.testdata/ilidata.xml.", Times.Once());
    }

    private void AssertModelsInterlisCh(Repository repository)
    {
        Assert.AreEqual("https://models.interlis.testdata/", repository.HostNameId);
        Assert.AreEqual(new Uri("https://models.interlis.testdata/"), repository.Uri);
        Assert.AreEqual("interlis.ch", repository.Name);
        Assert.AreEqual("Allgemeine technische INTERLIS-Modelle", repository.Title);
        Assert.AreEqual("Modell-Ablage des INTERLIS-Kernteams", repository.ShortDescription);
        Assert.AreEqual("http://www.interlis.ch", repository.Owner);
        Assert.AreEqual("mailto:info@interlis.ch", repository.TechnicalContact);
        repository.SubsidiarySites
            .AssertSingleItem(ss => "https://models.geo.admin.testdata/".Equals(ss.HostNameId, StringComparison.OrdinalIgnoreCase), AssertModelsGeoAdminCh)
            .AssertCount(2);
        repository.ParentSites.AssertCount(0);
        repository.Models
            .AssertCount(76)
            .AssertAllNotNull();
        repository.Catalogs
            .AssertCount(0);
    }

    private void AssertModelsGeoAdminCh(Repository repository)
    {
        Assert.AreEqual("https://models.geo.admin.testdata/", repository.HostNameId);
        Assert.AreEqual(new Uri("https://models.geo.admin.testdata/"), repository.Uri);
        Assert.AreEqual("models.geo.admin.ch", repository.Name);
        Assert.AreEqual("Geobasisdaten-Modell-Ablage", repository.Title);
        Assert.AreEqual("Modell-Ablage fuer Datenmodelle der Geobasisdaten des Bundesrechts", repository.ShortDescription);
        Assert.AreEqual("http://www.geo.admin.ch", repository.Owner);
        Assert.AreEqual("mailto:models@geo.admin.ch", repository.TechnicalContact);
        repository.SubsidiarySites
            .AssertCount(1)
            .AssertAllNotNull();
        repository.ParentSites
            .AssertCount(1)
            .AssertAllNotNull();
        repository.Models
            .AssertCount(1258)
            .AssertAllNotNull();
        repository.Catalogs
            .AssertCount(165)
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
        catalog.File
            .AssertContains("https://models.geo.admin.testdata/BLW/bearbeitungsstatus_kataloge_20140701.xml")
            .AssertCount(1);
        catalog.ReferencedModels
            .AssertContains("Biodiversitaetsfoerderflaechen_Qualitaetsstufe_II_und_Vernetzung_LV95_V1_3")
            .AssertContains("Bearbeitungsstatus_V1_2")
            .AssertCount(23)
            .AssertAllNotNull();
    }

    [TestMethod]
    public async Task CrawlerPerformsHttpsUpgrade()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Head, "https://models.interlis.testdata")
            .Respond(HttpStatusCode.OK);
        mockHttp
            .Expect("https://models.interlis.testdata/ilisite.xml")
            .Respond(HttpStatusCode.NotFound);
        mockHttp
            .Expect("https://models.interlis.testdata/ilimodels.xml")
            .Respond(HttpStatusCode.NotFound);
        mockHttp
            .Expect("https://models.interlis.testdata/ilidata.xml")
            .Respond(HttpStatusCode.NotFound);

        SetupRepositoryCrawlerInstance(mockHttp.ToHttpClient());
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("http://models.interlis.testdata"));

        result.AssertCount(0);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task CrawlerUsesFallbackUrlIfHttpsFails()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Head, "https://models.interlis.testdata")
            .Respond(HttpStatusCode.HttpVersionNotSupported);
        mockHttp
            .Expect("http://models.interlis.testdata/ilisite.xml")
            .Respond(HttpStatusCode.NotFound);
        mockHttp
            .Expect("http://models.interlis.testdata/ilimodels.xml")
            .Respond(HttpStatusCode.NotFound);
        mockHttp
            .Expect("http://models.interlis.testdata/ilidata.xml")
            .Respond(HttpStatusCode.NotFound);

        SetupRepositoryCrawlerInstance(mockHttp.ToHttpClient());
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("http://models.interlis.testdata"));

        result.AssertCount(0);
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task CrawlerSkipsMultipleOccurences()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.testdata"));
        result
            .AssertSingleItem("https://models.multiparent.testdata/", AssertModelsMultiparentCh)
            .AssertCount(3)
            .AssertAllNotNull();
    }

    private void AssertModelsMultiparentCh(Repository repository)
    {
        Assert.AreEqual("https://models.multiparent.testdata/", repository.HostNameId);
        Assert.AreEqual(new Uri("https://models.multiparent.testdata/"), repository.Uri);
        Assert.AreEqual("Minimal Multi Parent Repo", repository.Name);
        Assert.IsNull(repository.Title);
        Assert.IsNull(repository.ShortDescription);
        Assert.IsNull(repository.Owner);
        Assert.IsNull(repository.TechnicalContact);
        repository.SubsidiarySites
            .AssertCount(0);
        repository.ParentSites
            .AssertSingleItem(ps => "https://models.interlis.testdata/".Equals(ps.HostNameId, StringComparison.OrdinalIgnoreCase), AssertModelsInterlisCh)
            .AssertSingleItem(ps => "https://models.geo.admin.testdata/".Equals(ps.HostNameId, StringComparison.OrdinalIgnoreCase), AssertModelsGeoAdminCh)
            .AssertCount(2);
        repository.Models
            .AssertCount(3);
        repository.Catalogs
            .AssertCount(0);
    }

    [TestMethod]
    public async Task CrawlerDoesSupportBadFormattedXml()
    {
        mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When($"https://models.interlis.testdata/ilidata.xml")
            .Respond("application/xml", new MemoryStream(Encoding.UTF8.GetBytes("Bad Formatted xml stream")));
        mockHttp
            .When($"https://models.geo.admin.testdata/ilimodels.xml")
            .Respond("application/xml", new MemoryStream(Encoding.UTF8.GetBytes("Bad Formatted xml stream")));
        mockHttp
            .When($"https://models.multiparent.testdata/ilisite.xml")
            .Respond("application/xml", new MemoryStream(Encoding.UTF8.GetBytes("Bad Formatted xml stream")));

        SetupHttpMockFiles();
        SetupRepositoryCrawlerInstance(mockHttp.ToHttpClient());

        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.testdata"));
        result
            .AssertCount(2)
            .AssertAllNotNull();
    }

    [TestMethod]
    public async Task CrawlerCompletesMissingMD5()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.multiparent.testdata"));
        Assert.IsNotNull(result);
        result.AssertCount(1);

        result.AssertSingleItem("https://models.multiparent.testdata/", repository =>
        {
            repository.Models
                .AssertCount(3)
                .AssertSingleItem(m => m.Name == "Test_Model_Without_MD5", m => Assert.AreEqual("EB137F3B28D3D06C41F20237886A8B41", m.MD5))
                .AssertSingleItem(m => m.Name == "Test_Model_With_Empty_MD5", m => Assert.AreEqual("EB137F3B28D3D06C41F20237886A8B41", m.MD5))
                .AssertSingleItem(m => m.Name == "Test_Model_Without_MD5_And_Invalid_File", m => Assert.AreEqual(null, m.MD5));
        });
    }

    [TestMethod]
    public async Task ReplacedCatalogsReferencedByPrecurserVersionAreDiscarded()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.geo.admin.testdata"));

        result.AssertSingleItem("https://models.geo.admin.testdata/", repository =>
        {
            repository.Catalogs
                .AssertContainsNot(m => m.Identifier == "ch.admin.geo.models.UtilizzazionePrincipale_CH_V1_1" && m.Version == "2019-08-09")
                .AssertSingleItem(m => m.Identifier == "ch.admin.geo.models.UtilizzazionePrincipale_CH_V1_1", c =>
                {
                    Assert.AreEqual("2021-11-01", c.Version);
                    Assert.AreEqual("2019-08-09", c.PrecursorVersion);
                })
                .AssertContainsNot(m => m.Identifier == "ch.admin.geo.models.SafetyZonePlan_Catalogues_V1_3_20211125" && m.Version == "2021-11-25")
                .AssertSingleItem(m => m.Identifier == "ch.admin.geo.models.SafetyZonePlan_Catalogues_V1_3_20211125", c =>
                {
                    Assert.AreEqual("2022-02-04", c.Version);
                    Assert.AreEqual("2021-11-25", c.PrecursorVersion);
                })
                .AssertContainsNot(m => m.Identifier == "ch.admin.geo.models.OeREBKRM_V2_0_Texte_20220301" && m.Version == "2022-05-17")
                .AssertContainsNot(m => m.Identifier == "ch.admin.geo.models.OeREBKRM_V2_0_Texte_20220301" && m.Version == "2022-03-01")
                .AssertSingleItem(m => m.Identifier == "ch.admin.geo.models.OeREBKRM_V2_0_Texte_20220301", c =>
                {
                    Assert.AreEqual("2022-06-07", c.Version);
                    Assert.AreEqual("2022-05-17", c.PrecursorVersion);
                });
        });
    }
}
