using System.Diagnostics;
using LogNET;
using LogNET.Appenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    /// <summary>
    ///This is a test class for IanQvist.Logging.LogHelper.EventLogAppender and is intended
    ///to contain all IanQvist.Logging.LogHelper.EventLogAppender Unit Tests
    ///</summary>
    [TestClass]
    public class EventLogAppenderTest
    {
        /// <summary>
        ///A test for EventLogAppender (LogEntry)
        ///</summary>
        [TestMethod]
        public void EventLogAppender()
        {
            if (EventLog.SourceExists("Tests"))
                EventLog.DeleteEventSource("Tests");

            LogEntry logEntry = new LogEntry("Test Data", LogPriority.Information, new EventLogAppender(), false);
            logEntry.WriteToLog();

            Assert.AreEqual(true, EventLog.SourceExists("Tests"));
        }
    }
}