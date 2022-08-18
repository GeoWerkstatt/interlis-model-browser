﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Controllers;
using Moq;

namespace ModelRepoBrowser;

[TestClass]
public class ModelControllerTest
{
    private Mock<ILogger<ModelController>> loggerMock;
    private RepoBrowserContext context;
    private ModelController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        loggerMock = new Mock<ILogger<ModelController>>();
        context = ContextFactory.CreateContext();
        controller = new ModelController(loggerMock.Object, context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
        loggerMock.VerifyAll();
    }

    [TestMethod]
    public void ModelDetails()
    {
        var model = controller.ModelDetails("544a7e2f91f51172f1d471dc3b3ce10c", "Cotton_Officer");
        Assert.AreEqual("Cotton_Officer", model.Name);
        Assert.AreEqual("544a7e2f91f51172f1d471dc3b3ce10c", model.MD5);
        Assert.AreEqual("home/scalable_assistant_georgia.json5", model.File);
        Assert.IsNotNull(model.ModelRepository, "ModelRepository has to be included.");
        Assert.AreEqual("Practical Frozen Gloves", model.ModelRepository.Name);
    }

    [TestMethod]
    public void ModelDetailsNull()
    {
        Assert.AreEqual(null, controller.ModelDetails(null, null));
        Assert.AreEqual(null, controller.ModelDetails("f69ffd618a0998e13bc20b0e3026d19b", null));
        Assert.AreEqual(null, controller.ModelDetails(null, "Global_Licensed"));
    }

    [TestMethod]
    public void ModelDetailsEmptyString()
    {
        Assert.AreEqual(null, controller.ModelDetails(string.Empty, string.Empty));
        Assert.AreEqual(null, controller.ModelDetails("f69ffd618a0998e13bc20b0e3026d19b", string.Empty));
        Assert.AreEqual(null, controller.ModelDetails(string.Empty, "Global_Licensed"));
    }

    [TestMethod]
    public void ModelDetailsWhitespace()
    {
        Assert.AreEqual(null, controller.ModelDetails(" ", " "));
        Assert.AreEqual(null, controller.ModelDetails("f69ffd618a0998e13bc20b0e3026d19b", " "));
        Assert.AreEqual(null, controller.ModelDetails(" ", "Global_Licensed"));
    }

    [TestMethod]
    public void ModelDetailsOnlyFindsExactMatches()
    {
        Assert.AreEqual(null, controller.ModelDetails("544a7e2f91f5", "otton_Office"));
    }
}
