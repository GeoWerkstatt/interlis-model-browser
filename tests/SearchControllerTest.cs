using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Controllers;
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
        context = new RepoBrowserContext(new DbContextOptionsBuilder<RepoBrowserContext>().Options);
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
}
