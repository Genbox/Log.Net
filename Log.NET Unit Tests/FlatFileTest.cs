using System.IO;
using LogNET;
using LogNET.Appenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    /// <summary>
    ///This is a test class for IanQvist.Logging.LogHelper.FlatFileAppender and is intended
    ///to contain all IanQvist.Logging.LogHelper.FlatFileAppender Unit Tests
    ///</summary>
    [TestClass()]
    public class FlatFileAppenderTest
    {
        /// <summary>
        ///A test for FlatFileAppender (LogEntry)
        ///</summary>
        [TestMethod]
        public void FlatFileAppender()
        {
            LogEntry logEntry = new LogEntry("Test Data", LogPriority.Information, new FlatFileAppender(), false);
            logEntry.WriteToLog();

            Assert.AreEqual(true, File.Exists(logEntry.FullPath));
            File.Delete(logEntry.FullPath);
            Assert.AreEqual(false, File.Exists(logEntry.FullPath));
        }
    }
}