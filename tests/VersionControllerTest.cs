using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Controllers;

namespace ModelRepoBrowser;

[TestClass]
public class VersionControllerTest
{
    [TestMethod]
    public void Get()
    {
        var controller = new VersionController();
        Assert.AreEqual("1.0", controller.Get());
    }
}
