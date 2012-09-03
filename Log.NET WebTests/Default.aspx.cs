using System;
using System.Web.UI;
using LogNET;
using LogNET.Appenders;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LogEntry entry = new LogEntry("Test Data", LogPriority.Information, new EventLogAppender(), false);
        entry.WriteToLog();

        LogEntry entry1 = new LogEntry("Test Data", LogPriority.Information, new FlatFileAppender(), false);
        entry1.WriteToLog();

        LogEntry entry2 = new LogEntry("Test Data", LogPriority.Information, new XmlFileAppender(), false);
        entry2.WriteToLog();

        LogEntry entry3 = new LogEntry("Test Data", LogPriority.Information, new SmtpAppender(), false);
        entry3.WriteToLog();

        LogEntry entry4 = new LogEntry("Test Data", LogPriority.Information, new SqlServerAppender(), false);
        entry4.WriteToLog();

        using(new LogTracer("Testing of tracer"))
        {
            //whaaat? Yes, I'm empty!
        }
    }
}