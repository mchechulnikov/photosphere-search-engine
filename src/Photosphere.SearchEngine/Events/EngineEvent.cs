namespace Photosphere.SearchEngine.Events
{
    internal enum EngineEvent
    {
        PathWatchingStarted,
        PathWatchingEnded,
        FileIndexingStarted,
        FileIndexingEnded,
        FileRemovingStarted,
        FileRemovingEnded,
        FileUpdateInitiated,
        FileUpdateFailed,
        PathChanged,
        IndexCleanUpFailed
    }
}