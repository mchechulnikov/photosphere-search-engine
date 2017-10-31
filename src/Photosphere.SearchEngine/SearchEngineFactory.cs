using System;
using Photosphere.SearchEngine.Events;
using Photosphere.SearchEngine.FileIndexing;
using Photosphere.SearchEngine.FileParsing;
using Photosphere.SearchEngine.FileSupervision;
using Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching;
using Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers;
using Photosphere.SearchEngine.FileSupervision.FileSystemPolling;
using Photosphere.SearchEngine.FileVersioning;
using Photosphere.SearchEngine.FileVersioning.Services;
using Photosphere.SearchEngine.Scheduling;
using Photosphere.SearchEngine.Searching;

namespace Photosphere.SearchEngine
{
    /// <summary>
    /// Factory for creating search engine instance
    /// </summary>
    public static class SearchEngineFactory
    {
        /// <summary>
        /// Creates new search engine instance
        /// </summary>
        /// <returns>New search engine instance</returns>
        public static ISearchEngine New()
        {
            var settings = new SearchEngineSettings();
            return New(settings);
        }

        /// <summary>
        /// Creates new search engine instance
        /// </summary>
        /// <param name="settings">Engine settings object</param>
        /// <returns>New search engine instance</returns>
        public static ISearchEngine New(SearchEngineSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var eventReactor = new EventReactor();
            var fileParserProvider = new FileParserProvider(settings);
            var filesVersionsRegistry = new FilesVersionsRegistry();
            var index = new Index.Index();
            var indexer = new FileIndexer(eventReactor, fileParserProvider, index, filesVersionsRegistry, settings);
            var indexEjector = new IndexEjector(eventReactor, filesVersionsRegistry);
            var indexUpdater = new IndexUpdater(eventReactor, indexer, filesVersionsRegistry);
            var watchersCollection = new PathWatchersCollection();
            var fileSupervisor = new FileSupervisor(
                eventReactor,
                new FileSystemEventsProcessor(
                    new FileSystemEventHandler(
                        new CreateEventHandler(indexer),
                        new ChangeEventHandler(indexUpdater),
                        new DeleteEventHandler(eventReactor, indexEjector, watchersCollection),
                        new RenameEventHandler(
                            eventReactor,
                            new FilePathActualizer(filesVersionsRegistry),
                            watchersCollection
                        )
                    )
                ),
                watchersCollection,
                new PathPoller(
                    new DeadPathDetector(
                        watchersCollection,
                        new PathRemover(eventReactor, watchersCollection, indexEjector)
                    ),
                    watchersCollection
                )
            );
            var searcher = new Searcher(index);
            var indexCleaner = new IndexCleaner(eventReactor, index, filesVersionsRegistry, settings);
            var scheduler = new Scheduler(indexCleaner, settings);
            return new SearchEngine(eventReactor, indexer, indexEjector, fileSupervisor, searcher, scheduler);
        }
    }
}