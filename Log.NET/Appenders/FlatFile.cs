using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace LogNET.Appenders
{
    /// <summary>
    /// Writes to a flat file on the filesystem in the specified logpath from app.config or web.config
    /// </summary>
    public class FlatFileAppender : ILogAppender
    {
        public string GetPath(bool fallback = false)
        {
            string logPath = fallback ? ConfigurationManager.AppSettings["FallbackLogPath"] : ConfigurationManager.AppSettings["LogPath"];

            string filename = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) + ".txt";

            if (HttpContext.Current != null && (logPath.StartsWith("~", StringComparison.InvariantCulture) || logPath.StartsWith("/")))
                return HttpContext.Current.Server.MapPath(logPath + DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture) + @"\" + DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture) + @"\" + filename);

            return Path.GetFullPath(logPath) + DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture) + @"\" + DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture) + @"\" + filename;
        }

        public void Write(LogEntry entry)
        {
            string filename = GetPath();
            string path = Path.GetDirectoryName(filename);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (StreamWriter writer = new StreamWriter(filename, true, Encoding.Unicode))
                writer.Write(entry.ToString());
        }
    }
}