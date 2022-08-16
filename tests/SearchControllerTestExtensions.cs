using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelRepoBrowser.Models;
using ModelRepoBrowser.TestHelpers;

namespace ModelRepoBrowser;

public static class SearchControllerTestExtensions
{
    public static List<Model> GetAllModels(this Repository repository)
    {
        return repository.Models.Concat(repository.SubsidiarySites.SelectMany(s => GetAllModels(s))).ToList();
    }

    public static IEnumerable<Repository> AssertSingleItem(this IEnumerable<Repository> collection, string hostNameId, int modelCount, Action<IEnumerable<Repository>> assertChildren = null)
    {
        collection.AssertSingleItem(r => r.HostNameId == hostNameId,
            r =>
            {
                Assert.AreEqual(modelCount, r.Models.Count);

                Assert.IsNotNull(r.SubsidiarySites);
                assertChildren?.Invoke(r.SubsidiarySites);
            },
            "HostNameId: <{0}>.",
            hostNameId);

        return collection;
    }
}
