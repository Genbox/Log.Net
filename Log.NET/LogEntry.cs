using System;
using System.Configuration;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using LogNET.Appenders;

namespace LogNET
{
    /// <summary>
    /// The priority for the logentry
    /// </summary>
    public enum LogPriority
    {
        /// <summary>
        /// Used when the logentry is only reporting information
        /// </summary>
        Information,
        /// <summary>
        /// Used when a warning created by the application or potentially dangerous data is found
        /// </summary>
        Warning,
        /// <summary>
        /// Used when a critical error is detected
        /// </summary>
        Critical
    }

    /// <summary>
    /// Creates a logentry
    /// </summary>
    public class LogEntry
    {
        private string _subject;
        private string _userInput;
        private string _username;
        private string _userHostAddress;
        private string _location;
        private bool _encrypt;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="encrypt">if set to <c>true</c> [encrypt].</param>
        public LogEntry(string subject, string message, LogPriority priority, ILogAppender destination, bool encrypt)
        {
            _subject = subject;
            Init(message, priority, destination, encrypt);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="encrypt">if set to <c>true</c> [encrypt].</param>
        public LogEntry(string message, LogPriority priority, ILogAppender destination, bool encrypt)
        {
            Init(message, priority, destination, encrypt);
        }

        /// <summary>
        /// A quick constructor with the defaults:
        /// LogPriority = warning
        /// LogDestination = FlatFile
        /// Encrypt = false
        /// </summary>
        /// <param name="message">The log message</param>
        public LogEntry(string message)
        {
            Init(message, LogPriority.Warning, new FlatFileAppender(), false);
        }

        /// <summary>
        /// Shared method between constructors
        /// </summary>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        /// <param name="destination"></param>
        /// <param name="encrypt"></param>
        private void Init(string message, LogPriority priority, ILogAppender destination, bool encrypt)
        {
            Message = encrypt ? Encryption.Encrypt(message) : message;

            Priority = priority;

            Destination = destination;

            _encrypt = encrypt;

            if (HttpContext.Current != null)
                _username = HttpContext.Current.User.Identity.Name;
            else if (WindowsIdentity.GetCurrent().Name != null)
                _username = WindowsIdentity.GetCurrent().Name;
            else
                _username = "Unknown";

            if (HttpContext.Current != null)
                _location = HttpContext.Current.Request.RawUrl;
            else if (!string.IsNullOrEmpty(Assembly.GetCallingAssembly().GetName().FullName))
                _location = Assembly.GetCallingAssembly().GetName().FullName;
            else
                _location = "Unknown";

            Date = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get
            {
                if (!string.IsNullOrEmpty(_subject))
                    return _subject;
                
                return "none";
            }
            set { _subject = value; }
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public LogPriority Priority { get; set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get
            {
                if (!string.IsNullOrEmpty(_username))
                    return _username;
                
                try
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                catch (NullReferenceException)
                {
                    return "Cannot determine username";
                }
            }
        }

        /// <summary>
        /// Gets the user host address.
        /// </summary>
        /// <value>The user host address.</value>
        public string UserHostAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(_userHostAddress))
                    return _userHostAddress;
                
                try
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                catch (NullReferenceException)
                {
                    return "0.0.0.0";
                }
            }
            set { _userHostAddress = value; }
        }

        /// <summary>
        /// Gets or sets the user input.
        /// </summary>
        /// <value>The user input.</value>
        public string UserInput
        {
            get
            {
                if (!string.IsNullOrEmpty(_userInput))
                    return _userInput;
                
                return "";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _userInput = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Location
        {
            get
            {
                if (_location != null)
                    return _location;
                
                return "Unknown";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is encrypted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is encrypted; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncrypted
        {
            get { return _encrypt; }
            set
            {
                Message = value ? Encryption.Encrypt(Message) : Encryption.Decrypt(Message);
                _encrypt = value;
            }
        }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public ILogAppender Destination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fallback.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fallback; otherwise, <c>false</c>.
        /// </value>
        public bool IsFallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a tracer.
        /// </summary>
        /// <value><c>true</c> if this instance is tracer; otherwise, <c>false</c>.</value>
        public bool IsTracer { get; set; }

        /// <summary>
        /// Writes to log.
        /// </summary>
        public void WriteToLog()
        {
            try
            {
                Destination.Write(this);
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (ConfigurationManager.AppSettings["EnableFallBackLogs"] != null)
                {
                    bool isFallBackEnabled = bool.Parse(ConfigurationManager.AppSettings["EnableFallBackLogs"]);
                    if (isFallBackEnabled)
                    {
                        IsFallback = true;
                        Destination = new FlatFileAppender();
                        Message += "Traceback message: " + e.Message;
                        Destination.Write(this);
                    }
                    else
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new ConfigurationErrorsException("Please set EnableFallBackLogs to true or false in the configuration file.");
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            string fullMessage;

            if (IsTracer)
            {
                // This is the format of the tracer log entries:
                // [Date and time]
                // Application message: [Name / Message]
                fullMessage = Date + " " + "Message: " + Message + Environment.NewLine + Environment.NewLine;
            }
            else
            {
                // This is the format of the log entries:
                // [Date and time] - User: [UserName]
                // Priority: [Priority]
                // Message: [Message]
                // User input: [TextInput]
                // URI: [URI]
                fullMessage = Date + " - User: " + UserName + Environment.NewLine +
                              "Priority: " + Priority + Environment.NewLine +
                              "Subject: " + Subject + Environment.NewLine +
                              "Message: " + Message + Environment.NewLine +
                              "User input: " + UserInput + Environment.NewLine +
                              "Location: " + Location + Environment.NewLine + Environment.NewLine +
                              "HostAddress: " + UserHostAddress + Environment.NewLine + Environment.NewLine;
            }

            return fullMessage;
        }

        public string FullPath
        {
            get { return Destination.GetPath(); }
        }
    }
}