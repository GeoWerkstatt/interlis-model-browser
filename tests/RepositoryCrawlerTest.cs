using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Crawler;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRepoBrowser;

[TestClass]
public class RepositoryCrawlerTest
{
    private Mock<IHttpClientFactory> httpClientFactory;
    private IRepositoryCrawler repositoryCrawler;

    [TestInitialize]
    public void Initialize()
    {
        var mockHttp = new MockHttpMessageHandler();
        foreach (var dir in Directory.GetDirectories("./Testdata"))
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                mockHttp.When($"https://{Path.GetFileName(dir)}/{Path.GetFileName(file)}")
                    .Respond("application/xml", new FileStream(file, FileMode.Open));
            }
        }

        httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(cf => cf.CreateClient("")).Returns(mockHttp.ToHttpClient());

        repositoryCrawler = new RepositoryCrawler(NullLogger<RepositoryCrawler>.Instance, httpClientFactory.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        httpClientFactory.VerifyAll();
    }

    [TestMethod]
    public async Task CrawlerParsesIlisiteXml()
    {
        var result = await repositoryCrawler.CrawlModelRepositories(new Uri("https://models.interlis.ch"));
        Assert.IsNotNull(result);
    }
}
