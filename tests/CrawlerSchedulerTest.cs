using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelRepoBrowser
{
    [TestClass]
    public class CrawlerSchedulerTest
    {
        [TestMethod]
        public void GetTimeSpanUntilPreferedTime()
        {
            var crawlerScheduler = new CrawlerScheduler(new TimeOnly(16, 23));

            Assert.AreEqual(new TimeSpan(16, 23, 0), crawlerScheduler.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 0, 0, 0)));
            Assert.AreEqual(new TimeSpan(6, 8, 14), crawlerScheduler.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 10, 14, 46)));
            Assert.AreEqual(new TimeSpan(24, 0, 0), crawlerScheduler.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 16, 23, 0)));
            Assert.AreEqual(new TimeSpan(20, 54, 22), crawlerScheduler.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 19, 28, 38)));
        }
    }
}
