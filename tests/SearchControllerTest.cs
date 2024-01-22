using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Controllers;
using ModelRepoBrowser.TestHelpers;
using Moq;

namespace ModelRepoBrowser;

[TestClass]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Arrays only used once in test methods")]
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
    public async Task SearchNullOrEmpty()
    {
        var searchResult = await controller.Search("");
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(88); // gets all non obsolete Models.

        searchResult = await controller.Search(null);
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(88);
    }

    [TestMethod]
    public async Task SearchOneWhitespace()
    {
        var searchResult = await controller.Search(" "); // gets all non obsolete Models.
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(88);
    }

    [TestMethod]
    public async Task SearchName()
    {
        var searchResult = await controller.Search("entUCK");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(3)
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
        var searchResult = await controller.Search("open system");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(3)
            .AssertSingleItem(m => m.Id == 86, m => Assert.AreEqual("Handcrafted Granite Ball_Associate_haptic_Money Market Account_Beauty", m.Name))
            .AssertSingleItem(m => m.Id == 99, m => Assert.AreEqual("Bedfordshire", m.Name))
            .AssertSingleItem(m => m.Id == 43, m => Assert.AreEqual("Handcrafted Rubber Tuna_Sudanese Pound_syndicate", m.Name));
    }

    [TestMethod]
    public async Task SearchTag()
    {
        var searchResult = await controller.Search("Specialist");
        Assert.IsNotNull(searchResult);
        searchResult
            .GetAllModels()
            .AssertCount(4)
            .AssertSingleItem(m => m.Id == 23, m => m.Tags.AssertContains("Specialist"));

        // Tags must match exactly
        searchResult = await controller.Search("pecialist");
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(3);

        searchResult = await controller.Search("SpecialiST");
        Assert.IsNotNull(searchResult);
        searchResult.GetAllModels().AssertCount(3);
    }

    [TestMethod]
    public async Task SearchDependsOnModel()
    {
        var searchResult = await controller.Search("Home Loan Account");
        Assert.IsNotNull(searchResult);
        var models = searchResult.GetAllModels();
        models.Where(m => m.IsDependOnModelResult == true).AssertCount(2);
        models.Where(m => m.IsDependOnModelResult == false).AssertCount(4);
        models.AssertSingleItem(m => m.Id == 86, m => m.DependsOnModel.AssertContains("Home Loan Account"));

        // DependsOnModel must match exactly
        searchResult = await controller.Search("home loan account");
        searchResult!.GetAllModels().AssertCount(4);

        searchResult = await controller.Search("HOme Loan Account");
        searchResult!.GetAllModels().AssertCount(4);
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
        searchResult.GetAllModels().AssertCount(3);

        searchResult = await controller.Search("Kent_cky");
        Assert.AreEqual(null, searchResult);

        searchResult = await controller.Search("Ken%cky");
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
        searchResult.GetAllModels().AssertCount(21);
        Assert.AreEqual("delores.com", searchResult.HostNameId);
        Assert.AreEqual(1, searchResult.Models.Count);

        searchResult.SubsidiarySites
            .AssertCount(5)
            .AssertSingleItem("eldora.net", 2)
            .AssertSingleItem("geovany.org", 2)
            .AssertSingleItem("valentine.net", 0, r => r
                .AssertCount(7)
                .AssertSingleItem("arvel.name", 1)
                .AssertSingleItem("breana.com", 2)
                .AssertSingleItem("jaquelin.com", 3)
                .AssertSingleItem("lenny.net", 3)
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
                "back-end_grey_JBOD",
                "transition_vortals",
                "Iceland Krona_New Israeli Sheqel_matrix_Oklahoma",
                "Handcrafted Fresh Hat_metrics_invoice",
                "Handcrafted Rubber Tuna_Sudanese Pound_syndicate",
                "Iowa_Junctions",
                "Handcrafted Granite Ball_Associate_haptic_Money Market Account_Beauty",
                "bandwidth_auxiliary_Incredible",
                "violet_Metal_calculating",
                "maroon_synthesizing_Awesome",
                "index_transmitting_generate_24/7_Run",
                "Virgin Islands, U.S._withdrawal_CFA Franc BCEAO_THX",
                "capability_Unbranded Granite Table_Intelligent Cotton Table_static",
                "circuit_impactful_Organic",
                "SSL_cutting-edge_Global_platforms",
                "Interactions_convergence_static",
                "Via_West Virginia_withdrawal",
                "deposit",
                "Soft_Health_facilitate_Cotton",
                "bandwidth_Refined Fresh Shoes",
                "back-end_benchmark_Legacy_Future_Crescent",
                "North Dakota_bypass_Gorgeous Steel Keyboard",
                "full-range_Kenyan Shilling_experiences_Money Market Account_Officer",
                "connecting_Distributed_Florida_mission-critical_Awesome Wooden Bacon",
                "Global_Licensed",
                "Generic Plastic Cheese_Infrastructure_Intelligent_quantify",
                "IB_Health_sky blue",
                "Small_withdrawal_transition_Response_Response",
                "initiatives_Customer_neural_Bedfordshire_integrate",
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
