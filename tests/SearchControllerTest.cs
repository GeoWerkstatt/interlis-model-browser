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
        Assert.AreEqual(null, controller.EscapeLikePattern(null));
        Assert.AreEqual(string.Empty, controller.EscapeLikePattern(string.Empty));
        Assert.AreEqual("HOTRANGE", controller.EscapeLikePattern("HOTRANGE"));
        Assert.AreEqual(@"H\_OT\%AN\\GE", controller.EscapeLikePattern(@"H_OT%AN\GE"));
    }

    [TestMethod]
    public async Task SearchName()
    {
        var searchResult = await controller.Search("entUCK");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(2)
            .AssertSingleItem(m => m.Id == 22, m => Assert.AreEqual("Kentucky", m.Name));
    }

    [TestMethod]
    public async Task SearchVersion()
    {
        var searchResult = await controller.Search("021-02-2");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(2)
            .AssertSingleItem(m => m.Id == 89, m => Assert.AreEqual("2021-02-24", m.Version));
    }

    [TestMethod]
    public async Task SearchFilepath()
    {
        var searchResult = await controller.Search("spool/FANtastic");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(1)
            .AssertSingleItem(m => m.Id == 6, m => Assert.AreEqual("var/spool/fantastic.aab", m.File));
    }

    [TestMethod]
    public async Task SearchCatalog()
    {
        var searchResult = await controller.Search("irecTO");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(2)
            .AssertSingleItem(m => m.Id == 15, m => Assert.AreEqual("transition_vortals", m.Name))
            .AssertSingleItem(m => m.Id == 70, m => Assert.AreEqual("bandwidth_auxiliary_Incredible", m.Name));
    }

    [TestMethod]
    public async Task SearchTag()
    {
        var searchResult = await controller.Search("Specialist");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(3)
            .AssertSingleItem(m => m.Id == 23, m => m.Tags.AssertContains("Specialist"));

        // Tags must match exactly
        searchResult = await controller.Search("pecialist");
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(2);

        searchResult = await controller.Search("SpecialiST");
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(2);
    }

    [TestMethod]
    public async Task SearchDependsOnModel()
    {
        var searchResult = await controller.Search("Home Loan Account");
        Assert.IsNotNull(searchResult);
        var models = searchResult.GetAllModels();
        searchResult
            .GetAllModels()
            .AssertCount(3)
            .AssertSingleItem(m => m.Id == 86, m => m.DependsOnModel.AssertContains("Home Loan Account"));

        // DependsOnModel must match exactly
        searchResult = await controller.Search("agenta");
        Assert.AreEqual(null, searchResult);

        searchResult = await controller.Search("MAgenta");
        Assert.AreEqual(null, searchResult);
    }

    [TestMethod]
    public async Task SearchOmitsObsolete()
    {
        context.Models
            .Where(m => m.File.StartsWith("obsolete"))
            .ToList()
            .AssertCount(12, "Precondition: the testdata contains obsolete models.");

        var searchResult = await controller.Search("obsolete");
        Assert.AreEqual(null, searchResult);
    }

    [TestMethod]
    public async Task SearchWithWildcardsDisabled()
    {
        var searchResult = await controller.Search("Kentucky");
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(2);

        searchResult = await controller.Search("Kent_cky");
        Assert.AreEqual(null, searchResult);

        searchResult = await controller.Search("Ken%cky");
        Assert.AreEqual(null, searchResult);
    }

    [TestMethod]
    public async Task SearchEmptyString()
    {
        var searchResult = await controller.Search(string.Empty);
        Assert.AreEqual(null, searchResult);
    }

    [TestMethod]
    public async Task SearchNull()
    {
        var searchResult = await controller.Search(null);
        Assert.AreEqual(null, searchResult);
    }

    [TestMethod]
    public async Task SearchOneWhitespace()
    {
        var searchResult = await controller.Search(" ");
        Assert.AreEqual(null, searchResult);
    }

    [TestMethod]
    public async Task SearchQueryWithSpace()
    {
        var searchResult = await controller.Search("ken tucky");
        Assert.AreEqual(null, searchResult);
    }

    [TestMethod]
    public async Task SearchQueryWrittenToDatabase()
    {
        await controller.Search("reinvent_maroon_Washington_connect_clear-thinking");

        context.SearchQueries.Select(s => s.Query).AssertContains("reinvent_maroon_Washington_connect_clear-thinking");
    }

    [TestMethod]
    public async Task SearchRepositoryTreeResult()
    {
        var searchResult = await controller.Search("ga");

        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(11);
        Assert.AreEqual("delores.com", searchResult.HostNameId);
        Assert.AreEqual(1, searchResult.Models.Count);

        searchResult.SubsidiarySites
            .AssertCount(5)
            .AssertSingleItem("eldora.net", 1)
            .AssertSingleItem("geovany.org", 1)
            .AssertSingleItem("valentine.net", 0, r => r
                .AssertCount(5)
                .AssertSingleItem("arvel.name", 1)
                .AssertSingleItem("breana.com", 1)
                .AssertSingleItem("jaquelin.com", 2)
                .AssertSingleItem("lenny.net", 1)
                .AssertSingleItem("mack.info", 1));
    }

    [TestMethod]
    public async Task SearchNameFilterByRepositoryName()
    {
        var searchResult = await controller.Search("entUCK", new string[] { "relationships" });
        Assert.IsNotNull(searchResult);
        Assert.AreEqual(searchResult.SubsidiarySites.Single().SubsidiarySites.Single().Name, "relationships");
        searchResult.GetAllModels().AssertCount(1)
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("deposit", m.Name))
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("Daniela.Hand26@hotmail.com", m.Issuer))
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("opt/include/specialist_down_sized_kentucky.teicorpus", m.File))
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("Berkshire hack Georgia", m.ShortDescription));
    }

    [TestMethod]
    public async Task SearchNameFilterByIssuer()
    {
        var searchResult = await controller.Search("entUCK", null, new string[] { "Alysa_Bahringer@hotmail.com" });
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(1)
        .AssertSingleItem(m => m.Id == 22, m => Assert.AreEqual("Kentucky", m.Name))
        .AssertSingleItem(m => m.Id == 22, m => Assert.AreEqual("Alysa_Bahringer@hotmail.com", m.Issuer));
    }

    [TestMethod]
    public async Task SearchNameFilterSchemaLanguage()
    {
        var searchResult = await controller.Search("entUCK", null, null, new string[] { "ili2_3" });
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(1)
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("deposit", m.Name))
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("Daniela.Hand26@hotmail.com", m.Issuer))
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("opt/include/specialist_down_sized_kentucky.teicorpus", m.File))
        .AssertSingleItem(m => m.Id == 27, m => Assert.AreEqual("Berkshire hack Georgia", m.ShortDescription));
    }

    [TestMethod]
    public async Task SearchNameFilterByDependsOnModels()
    {
        var searchResult = await controller.Search("ent", null, null, null, new string[] { "synthesizing" });
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(1)
        .AssertSingleItem(m => m.Id == 53, m => Assert.AreEqual("capability_Unbranded Granite Table_Intelligent Cotton Table_static", m.Name))
        .AssertSingleItem(m => m.Id == 53, m => Assert.AreEqual("Reverse-engineered Lead orchid", m.ShortDescription))
        .AssertSingleItem(m => m.Id == 53, m => Assert.AreEqual("synthesizing", m.DependsOnModel[1]));
    }

    [TestMethod]
    public async Task GetSearchSuggestions()
    {
        const string query = "and";
        var suggestions = await controller.GetSearchSuggestions(query);
        var search = await controller.Search(query);
        Assert.IsNotNull(search);

        CollectionAssert.AreEquivalent(new[]
            {
                "Handcrafted Rubber Tuna_Sudanese Pound_syndicate",
                "Engineer_Hill_Solutions_Practical_Michigan",
                "Iceland Krona_New Israeli Sheqel_matrix_Oklahoma",
                "Handcrafted Granite Ball_Associate_haptic_Money Market Account_Beauty",
                "deposit",
                "Virgin Islands, U.S._withdrawal_CFA Franc BCEAO_THX",
                "indigo_Sleek Granite Salad_Practical Concrete Ball_moderator_interface",
                "back-end_benchmark_Legacy_Future_Crescent",
                "capability_Unbranded Granite Table_Intelligent Cotton Table_static",
                "Global_Licensed",
                "bandwidth_Refined Fresh Shoes",
                "back-end_grey_JBOD",
                "bandwidth_auxiliary_Incredible",
                "Handcrafted Fresh Hat_metrics_invoice",
                "Iowa_Junctions",
                "Via_West Virginia_withdrawal",
            },
            suggestions.ToArray());

        CollectionAssert.AreEquivalent(search.GetAllModels().Select(x => x.Name).ToList(), suggestions.ToList());
    }

    [TestMethod]
    public async Task GetSearchSuggestionsWithSchemaLanguageFilter()
    {
        const string query = "and";
        var suggestions = await controller.GetSearchSuggestions(query, null, null, new string[] { "ili2_3" });
        var search = await controller.Search(query);
        Assert.IsNotNull(search);

        CollectionAssert.AreEquivalent(new[]
            {
            "back-end_benchmark_Legacy_Future_Crescent",
            "deposit",
            "Global_Licensed",
            "bandwidth_Refined Fresh Shoes",
            },
            suggestions.ToArray());
    }

    [TestMethod]
    public async Task GetSearchSuggestionsNull()
    {
        var suggestions = await controller.GetSearchSuggestions(null);
        suggestions.AssertCount(0);
    }

    [TestMethod]
    public async Task GetSearchSuggestionsEmptyString()
    {
        var suggestions = await controller.GetSearchSuggestions(string.Empty);
        suggestions.AssertCount(0);
    }

    [TestMethod]
    public async Task GetSearchSuggestionsWhitespace()
    {
        var suggestions = await controller.GetSearchSuggestions(" ");
        suggestions.AssertCount(0);
    }
}
