using System.Configuration;
using System.Diagnostics;

namespace LogNET.Appenders
{
    /// <summary>
    /// Writes to the servers own eventlog
    /// </summary>
    public class EventLogAppender : ILogAppender
    {
        public string GetPath(bool fallback = false)
        {
            return "127.0.0.1";
        }

        public void Write(LogEntry entry)
        {
            using (EventLog log = new EventLog())
            {
                if (ConfigurationManager.AppSettings["ApplicationName"] != null)
                {
                    log.Source = ConfigurationManager.AppSettings["ApplicationName"];

                    switch (entry.Priority)
                    {
                        case LogPriority.Information:
                            log.WriteEntry(entry.ToString(), EventLogEntryType.Information);
                            break;
                        case LogPriority.Warning:
                            log.WriteEntry(entry.ToString(), EventLogEntryType.Warning);
                            break;
                        case LogPriority.Critical:
                            log.WriteEntry(entry.ToString(), EventLogEntryType.Error);
                            break;
                    }
                }
                else
                {
                    throw new ConfigurationErrorsException("Please provide an application name in the configuration file");
                }
            }
        }
    }
}