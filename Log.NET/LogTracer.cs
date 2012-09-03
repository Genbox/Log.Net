using System;
using LogNET.Appenders;

namespace LogNET
{
    /// <summary>
    /// Creates a tracer that logs whenever an object starts and ends
    /// </summary>
    public sealed class LogTracer : IDisposable
    {
        private string _name;
        private LogEntry _entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTracer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public LogTracer(string name)
        {
            _name = name;
            _entry = new LogEntry("starting: " + _name, LogPriority.Information, new FlatFileAppender(), false);
            _entry.IsTracer = true;
            _entry.WriteToLog();
        }

        public string FullPath
        {
            get { return _entry.FullPath; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _entry = new LogEntry("shutting down: " + _name, LogPriority.Information, new FlatFileAppender(), false);
            _entry.IsTracer = true;
            _entry.WriteToLog();
        }
    }
}