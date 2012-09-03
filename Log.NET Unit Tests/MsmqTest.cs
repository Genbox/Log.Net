using System;
using System.Configuration;
using System.Messaging;
using LogNET;
using LogNET.Appenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    /// <summary>
    ///This is a test class for IanQvist.Logging.LogHelper.MsmqAppender and is intended
    ///to contain all IanQvist.Logging.LogHelper.MsmqAppender Unit Tests
    ///</summary>
    [TestClass()]
    public class MsmqAppenderTest
    {
        /// <summary>
        ///A test for MsmqAppender (LogEntry)
        ///</summary>
        [TestMethod]
        public void MsmqAppender()
        {
            LogEntry logEntry = new LogEntry("Test", LogPriority.Warning, new MsmqAppender(), false);
            logEntry.WriteToLog();

            using (MessageQueue mq = new MessageQueue(ConfigurationManager.AppSettings["MsmqPath"]))
            {
                try
                {
                    Message message = mq.Receive();
                    Assert.AreEqual(message.Body, "Test");
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}