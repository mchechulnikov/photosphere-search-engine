namespace Jbta.SearchEngine.Events
{
    internal enum EngineEvent
    {
        FileIndexingStarted,
        FileIndexingEnded,
        FileRemovingStarted,
        FileRemovingEnded,
        FileUpdateInitiated,
        FileUpdateFailed,
        FilePathChanged,
        IndexCleanUpFailed
    }
}