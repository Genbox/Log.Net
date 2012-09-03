using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace LogNET.Appenders
{
    /// <summary>
    /// Writes to a XML file in the specifed logpath from app.config or web.config
    /// </summary>
    public class XmlFileAppender : ILogAppender
    {
        public string GetPath(bool fallback = false)
        {
            string logPath = fallback ? ConfigurationManager.AppSettings["FallbackLogPath"] : ConfigurationManager.AppSettings["LogPath"];

            if (!string.IsNullOrEmpty(logPath))
            {
                string filename = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) + ".xml";

                if (HttpContext.Current != null && (logPath.StartsWith("~", StringComparison.InvariantCulture) || logPath.StartsWith("/", StringComparison.InvariantCulture)))
                    return HttpContext.Current.Server.MapPath(logPath + DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture) + @"\" + DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture) + @"\" + filename);

                return Path.GetFullPath(logPath) + DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture) + @"\" + DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture) + @"\" + filename;
            }

            throw new ConfigurationErrorsException("You need to configure a logpath in the configuration file");
        }

        public void Write(LogEntry entry)
        {
            string filename = GetPath();
            string path = Path.GetDirectoryName(filename);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.Load(filename);
            }
            catch (FileNotFoundException)
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(filename, Encoding.UTF8))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteComment("Log file - do not edit by hand");
                    xmlWriter.WriteStartElement("LogEntries");
                }
                xmlDoc.Load(filename);
            }

            XmlNode root = xmlDoc.DocumentElement;
            XmlElement xmlLogEntry = xmlDoc.CreateElement("LogEntry");

            XmlAttribute date = xmlDoc.CreateAttribute("Date");
            date.InnerText = entry.Date.ToString();

            XmlAttribute priority = xmlDoc.CreateAttribute("Priority");
            priority.InnerText = entry.Priority.ToString();

            XmlElement xmlLogEntryMessage = xmlDoc.CreateElement("Message");
            xmlLogEntryMessage.InnerText = entry.Message;

            XmlElement xmlLogEntryUserInput = xmlDoc.CreateElement("Userinput");
            xmlLogEntryUserInput.InnerText = entry.UserInput;

            if (entry.IsTracer)
            {
                // This is the format of the tracer log entries:
                // <logentry date=[Date and time] type=tracer>
                //      <message>
                //          [Message]
                //      </message>
                // </logentry>
                xmlLogEntry.Attributes.Append(date);
                xmlLogEntry.AppendChild(xmlLogEntryMessage);
            }
            else
            {
                // This is the format of the log entries:
                // <logentry date=[Date and time] priority=[Priority]>
                //      <message>
                //          [Message]
                //      </message>
                //      <userinput uri=[URI]>
                //          [TextInput]
                //      </userinput>
                // </logentry>
                xmlLogEntry.Attributes.Append(date);
                xmlLogEntry.Attributes.Append(priority);
                xmlLogEntry.AppendChild(xmlLogEntryMessage);

                XmlAttribute uri = xmlDoc.CreateAttribute("URI");
                uri.InnerText = entry.Location;

                xmlLogEntryUserInput.Attributes.Append(uri);
                xmlLogEntry.AppendChild(xmlLogEntryUserInput);
            }

            root.AppendChild(xmlLogEntry);
            xmlDoc.Save(filename);
        }
    }
}