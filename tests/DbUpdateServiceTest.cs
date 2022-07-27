using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelRepoBrowser
{
    [TestClass]
    public class DbUpdateServiceTest
    {
        [TestMethod]
        public void GetTimeSpanUntilPreferedTime()
        {
            Assert.AreEqual(new TimeSpan(16, 23, 0), DbUpdateService.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 0, 0, 0), new TimeOnly(16, 23)));
            Assert.AreEqual(new TimeSpan(6, 8, 14), DbUpdateService.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 10, 14, 46), new TimeOnly(16, 23)));
            Assert.AreEqual(new TimeSpan(24, 0, 0), DbUpdateService.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 16, 23, 0), new TimeOnly(16, 23)));
            Assert.AreEqual(new TimeSpan(20, 54, 22), DbUpdateService.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 19, 28, 38), new TimeOnly(16, 23)));
            Assert.AreEqual(new TimeSpan(4, 31, 22), DbUpdateService.GetTimeSpanUntilPreferedTime(new DateTime(2022, 7, 26, 19, 28, 38), new TimeOnly(0, 0)));
        }
    }
}
