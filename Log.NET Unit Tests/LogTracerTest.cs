using System.IO;
using LogNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    /// <summary>
    ///This is a test class for IanQvist.Logging.LogHelper.LogTracer and is intended
    ///to contain all IanQvist.Logging.LogHelper.LogTracer Unit Tests
    ///</summary>
    [TestClass()]
    public class LogTracerTest
    {
        /// <summary>
        ///A test for LogTracer (string)
        ///</summary>
        [TestMethod()]
        public void LogTracer()
        {
            const string name = "Tracer";
            LogTracer target;

            using (target = new LogTracer(name))
            {
                // Do nothing
            }

            Assert.AreEqual(true, File.Exists(target.FullPath));

            File.Delete(target.FullPath);

            Assert.AreEqual(false, File.Exists(target.FullPath));

        }
    }
}