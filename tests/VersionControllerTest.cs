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
        StringAssert.StartsWith(controller.Get(), "1.0", StringComparison.Ordinal);
    }
}
