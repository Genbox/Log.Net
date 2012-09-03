using System.IO;
using LogNET;
using LogNET.Appenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    [TestClass]
    public class XmlFileAppenderTest
    {
        /// <summary>
        ///A test for XmlFileAppender (LogEntry)
        ///</summary>
        [TestMethod()]
        public void XmlFileAppender()
        {
            LogEntry logEntry = new LogEntry("Test Data", LogPriority.Information, new XmlFileAppender(), false);
            logEntry.WriteToLog();

            Assert.AreEqual(true, File.Exists(logEntry.FullPath), "XML file does not exist");

            // Clean up
            File.Delete(logEntry.FullPath);

            Assert.AreEqual(false, File.Exists(logEntry.FullPath), "XML file does still exist");
        }
    }
}