using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelRepoBrowser;

[TestClass]
public class StringExtensionsTest
{
    [TestMethod]
    [DataRow((string?)null)]
    [DataRow("")]
    [DataRow("Test input")]
    [DataRow("Test input with special characters: äöü?")]
    public void EscapeWithoutNewLines(string? input)
    {
        var result = input.EscapeNewLines();

        Assert.AreEqual(input, result);
    }

    [TestMethod]
    [DataRow("\n", " ")]
    [DataRow("Test\r\ninput", "Test input")]
    [DataRow("Test\n\n\ninput\r\r", "Test   input  ")]
    [DataRow("Test input\rwith special\ncharacters:\r\näöü?", "Test input with special characters: äöü?")]
    public void EscapeWithNewLines(string? input, string? expected)
    {
        var result = input.EscapeNewLines();

        Assert.AreEqual(expected, result);
    }
}
