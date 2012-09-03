using System.Configuration;
using System.Data.SqlClient;
using LogNET;
using LogNET.Appenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    /// <summary>
    ///This is a test class for IanQvist.Logging.LogHelper.SqlServerAppender and is intended
    ///to contain all IanQvist.Logging.LogHelper.SqlServerAppender Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerAppenderTest
    {
        /// <summary>
        ///A test for SqlServerAppender (LogEntry)
        ///</summary>
        [TestMethod]
        public void SqlServerAppender()
        {
            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["LoggingServer"].ConnectionString);
            sqlConn.Open();

            SqlCommand sqlDelete = new SqlCommand("DELETE FROM Logs", sqlConn);
            sqlDelete.ExecuteNonQuery();

            sqlConn.Close();

            LogEntry logEntry = new LogEntry("Test Data", LogPriority.Information, new SqlServerAppender(), false);
            logEntry.WriteToLog();

            sqlConn.Open();
            
            SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Logs", sqlConn);
            SqlDataReader rdr = sqlCmd.ExecuteReader();

            Assert.AreEqual(true, rdr.HasRows);
            
            while (rdr.Read())
            {
                Assert.AreEqual((string)rdr["LogMessage"], "Test Data", "The log message was not Test Data");
            }
            sqlConn.Close();
        }
    }
}