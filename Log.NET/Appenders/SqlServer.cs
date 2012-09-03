using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LogNET.Appenders
{
    /// <summary>
    /// Writes to a mssql SQL server on the network using the specified connectionstring from app.config or web.config
    /// </summary>
    public class SqlServerAppender : ILogAppender
    {
        private SqlCommand _sqlCmd;

        private void CreateDatabase()
        {
            _sqlCmd.CommandText = "SELECT id FROM Logs";
            try
            {
                using (SqlDataReader sqlRdr = _sqlCmd.ExecuteReader())
                {
                    //Leave empty
                }
            }
            catch (SqlException e)
            {
                if (e.Message == "Invalid object name 'Logs'.")
                {
                    _sqlCmd.CommandText =
                        "CREATE TABLE [dbo].[Logs]([Id] [int] IDENTITY(1,1) NOT NULL,[LogDate] [datetime] NOT NULL,[LogSubject] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,[LogMessage] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,[LogPriority] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,[LogUserName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,[LogUri] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,[LogUserInput] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL, [LogUserAddress] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                    _sqlCmd.ExecuteNonQuery();
                }
            }
        }

        private void WriteLogEntry(LogEntry logEntry)
        {
            _sqlCmd.Parameters.AddWithValue("@LogDate", SqlDbType.DateTime).Value = logEntry.Date;
            _sqlCmd.Parameters.AddWithValue("@LogSubject", SqlDbType.NVarChar).Value = logEntry.Subject;
            _sqlCmd.Parameters.AddWithValue("@LogMessage", SqlDbType.NText).Value = logEntry.Message;
            _sqlCmd.Parameters.AddWithValue("@LogPriority", SqlDbType.NVarChar).Value = logEntry.Priority.ToString();
            _sqlCmd.Parameters.AddWithValue("@LogUserName", SqlDbType.NVarChar).Value = logEntry.UserName;
            _sqlCmd.Parameters.AddWithValue("@LogUri", SqlDbType.NVarChar).Value = logEntry.Location;
            _sqlCmd.Parameters.AddWithValue("@LogUserInput", SqlDbType.NText).Value = logEntry.UserInput;
            _sqlCmd.Parameters.AddWithValue("@LogUserAddress", SqlDbType.NVarChar).Value = logEntry.UserHostAddress;

            _sqlCmd.CommandText = "INSERT INTO Logs (LogDate,LogSubject,LogMessage,LogPriority,LogUserName,LogUri,LogUserInput,LogUserAddress) VALUES(@LogDate,@LogSubject,@LogMessage,@LogPriority,@LogUserName,@LogUri,@LogUserInput,@LogUserAddress)";
            _sqlCmd.ExecuteNonQuery();
        }

        public string GetPath(bool fallback = false)
        {
            if (ConfigurationManager.ConnectionStrings["LoggingServer"] != null)
                return ConfigurationManager.ConnectionStrings["LoggingServer"].ConnectionString;

            throw new ConfigurationErrorsException("Please configure the LoggingServer connection string in the configuration file");
        }

        public void Write(LogEntry entry)
        {
            using (SqlConnection sqlConn = new SqlConnection(GetPath()))
            {
                sqlConn.Open();
                using (_sqlCmd = new SqlCommand())
                {
                    _sqlCmd.Connection = sqlConn;

                    CreateDatabase();

                    WriteLogEntry(entry);
                }
            }
        }
    }
}