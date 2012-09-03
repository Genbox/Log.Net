# Log.NET - A modular logger written in C-sharp

### Features

* Support for writing to different outputs: XML, flat files, MSSQL database, eventlog and MSMQ
* Support for fallback logs. It writes to a file on the local drive if the log fails.
* Log tracers that logs when it starts and ends logging
* Encryption support

### Examples

Here is how you log to a file:
Note: Be sure to setup the App.config file correctly. An App.config.example is provided with the project.

```csharp
static void Main(string[] args)
{
	LogEntry entry = new LogEntry("Test data with special characters !\"#%&/()=?`¨'רזו,.", LogPriority.Information, new FlatFileAppender(), true);
	entry.WriteToLog();
}
```

For more examples, take a look at the Log.NET unit tests included in the project.