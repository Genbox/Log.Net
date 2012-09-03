# Log.NET - A modular logger written in C#

### Features

* Support for writing to different outputs:
* XML files
* Flat files
* MSSQL database
* Eventlog
* MSMQ

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