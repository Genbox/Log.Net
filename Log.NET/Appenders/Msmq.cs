using System.Configuration;
using System.Messaging;

namespace LogNET.Appenders
{
    /// <summary>
    /// Writes to a MSMQ server in the specified path from the app.config or web.config file
    /// </summary>
    public class MsmqAppender : ILogAppender
    {
        public string GetPath(bool fallback = false)
        {
            if (ConfigurationManager.AppSettings["MsmqPath"] != null)
                return ConfigurationManager.AppSettings["MsmqPath"];

            throw new ConfigurationErrorsException("Please configure MsmqPath in the configuration file");
        }

        public void Write(LogEntry entry)
        {
            using (MessageQueue mq = new MessageQueue(ConfigurationManager.AppSettings["MsmqPath"]))
            {
                Message msmqMessage = new Message();

                msmqMessage.Body = entry.ToString();

                msmqMessage.Formatter = new XmlMessageFormatter(new[] { "System.String,mscorlib" });
                mq.Send(msmqMessage);
            }
        }
    }
}