using System;
using System.Runtime.InteropServices;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileSupervision;
using Jbta.SearchEngine.FileSupervision.FileSystemEventWatching;
using Jbta.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers;
using Jbta.SearchEngine.FileSupervision.FileSystemPolling;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.FileVersioning.Services;
using Jbta.SearchEngine.Scheduling;
using Jbta.SearchEngine.Searching;

namespace Jbta.SearchEngine
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