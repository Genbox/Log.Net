using LogNET;
using LogNET.Appenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    /// <summary>
    ///This is a test class for IanQvist.Logging.LogHelper.SmtpAppender and is intended
    ///to contain all IanQvist.Logging.LogHelper.SmtpAppender Unit Tests
    ///</summary>
    [TestClass()]
    public class SmtpAppenderTest
    {
        /// <summary>
        ///A test for SmtpAppender (LogEntry)
        ///</summary>
        [TestMethod]
        public void SmtpAppender()
        {
            LogEntry logEntry = new LogEntry("Test Data", LogPriority.Information, new SmtpAppender(), false);
            logEntry.WriteToLog();
        }
    }
}