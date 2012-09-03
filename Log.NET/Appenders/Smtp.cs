using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace LogNET.Appenders
{
    /// <summary>
    /// Writes an email to the specified email using the server credentials from app.config or web.config
    /// </summary>
    public class SmtpAppender : ILogAppender
    {
        public string GetPath(bool fallback = false)
        {
            if (ConfigurationManager.AppSettings["ToEmail"] != null)
            {
                return ConfigurationManager.AppSettings["ToEmail"];
            }

            throw new ConfigurationErrorsException("Please configure SmtpEmail in the configuration file");
        }

        public void Write(LogEntry entry)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.Body = entry.ToString();
                mailMessage.Subject = entry.Priority + " Log message from " +
                                      ConfigurationManager.AppSettings["ApplicationName"];
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"]);
                mailMessage.To.Add(new MailAddress(GetPath()));

                SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                string username = ConfigurationManager.AppSettings["SmtpUsername"];
                string password = ConfigurationManager.AppSettings["SmtpPassword"];

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    client.Credentials = new NetworkCredential(username, password);
                }
                client.Send(mailMessage);
            }
        }
    }
}