namespace LogNET
{
    public interface ILogAppender
    {
        string GetPath(bool fallback = false);
        void Write(LogEntry entry);
    }
}
