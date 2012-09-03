using System;
using LogNET;
using LogNET.Appenders;

namespace LogNETManualUnitTests
{
    class Program
    {
        static void Main()
        {
            LogEntry entry = new LogEntry("Test data with special characters !\"#%&/()=?`¨'רזו,.", LogPriority.Information, new FlatFileAppender(), true);
            Console.WriteLine(entry.Message);
            Console.Read();
        }
    }
}
