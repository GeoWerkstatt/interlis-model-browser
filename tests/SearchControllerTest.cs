using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Controllers;
using ModelRepoBrowser.TestHelpers;
using Moq;

namespace ModelRepoBrowser;

[TestClass]
public class SearchControllerTest
{
    private Mock<ILogger<SearchController>> loggerMock;
    private RepoBrowserContext context;
    private SearchController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        loggerMock = new Mock<ILogger<SearchController>>();
        context = ContextFactory.CreateContext();
        controller = new SearchController(loggerMock.Object, context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public void EscapeLikePattern()
    {
        Assert.AreEqual("HOTRANGE", controller.EscapeLikePattern("HOTRANGE"));
        Assert.AreEqual(@"H\_OT\%AN\\GE", controller.EscapeLikePattern(@"H_OT%AN\GE"));
    }

    [TestMethod]
    public async Task SearchName()
    {
        var searchResult = await controller.Search("entUCK");
        searchResult
            .AssertCount(2)
            .AssertSingleItem(m => m.Id == 22, m => Assert.AreEqual("Kentucky", m.Name));
    }

    [TestMethod]
    public async Task SearchVersion()
    {
        var searchResult = await controller.Search("021-02-2");
        searchResult
            .AssertCount(2)
            .AssertSingleItem(m => m.Id == 89, m => Assert.AreEqual("2021-02-24", m.Version));
    }

    [TestMethod]
    public async Task SearchFilepath()
    {
        var searchResult = await controller.Search("spool/FANtastic");
        searchResult
            .AssertCount(1)
            .AssertSingleItem(m => m.Id == 6, m => Assert.AreEqual("var/spool/fantastic.aab", m.File));
    }

    [TestMethod]
    public async Task SearchCatalog()
    {
        var searchResult = await controller.Search("irecTO");
        searchResult
            .AssertCount(2)
            .AssertSingleItem(m => m.Id == 15, m => Assert.AreEqual("transition_vortals", m.Name))
            .AssertSingleItem(m => m.Id == 70, m => Assert.AreEqual("bandwidth_auxiliary_Incredible", m.Name));
    }

    [TestMethod]
    public async Task SearchTag()
    {
        var searchResult = await controller.Search("Centralized");
        searchResult
            .AssertCount(1)
            .AssertSingleItem(m => m.Id == 23, m => m.Tags.AssertContains("Centralized"));

        // tags must match exactly
        searchResult = await controller.Search("entralize");
        searchResult.AssertCount(0);

        searchResult = await controller.Search("centralizED");
        searchResult.AssertCount(0);
    }

    [TestMethod]
    public async Task SearchOmitsObsolete()
    {
        context.Models
            .Where(m => m.File.StartsWith("obsolete"))
            .ToList()
            .AssertCount(12, "Precondition: the testdata contains obsolete models.");

        var searchResult = await controller.Search("obsolete");
        searchResult.AssertCount(0);
    }

    [TestMethod]
    public async Task SearchWithWildcardsDisabled()
    {
        var searchResult = await controller.Search("Kentucky");
        searchResult.AssertCount(2);

        searchResult = await controller.Search("Kent_cky");
        searchResult.AssertCount(0);

        searchResult = await controller.Search("Ken%cky");
        searchResult.AssertCount(0);
    }

    [TestMethod]
    public async Task SearchQueryWrittenToDatabase()
    {
        await controller.Search("reinvent_maroon_Washington_connect_clear-thinking");

        context.SearchQueries.Select(s => s.Query).AssertContains("reinvent_maroon_Washington_connect_clear-thinking");
    }
}
