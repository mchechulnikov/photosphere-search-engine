using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileSupervision;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.FileVersioning.Services;
using Jbta.SearchEngine.Scheduling;
using Jbta.SearchEngine.Searching;

namespace Jbta.SearchEngine
{
    public static class SearchEngineFactory
    {
        public static ISearchEngine New()
        {
            var settings = new SearchEngineSettings();
            return New(settings);
        }

        public static ISearchEngine New(SearchEngineSettings settings)
        {
            var eventReactor = new EventReactor();
            var fileParserProvider = new FileParserProvider(settings);
            var filesVersionsRegistry = new FilesVersionsRegistry(eventReactor);
            var index = new Index.Index();
            var indexer = new FileIndexer(eventReactor, fileParserProvider, index, filesVersionsRegistry, settings);
            var indexEjector = new IndexEjector(eventReactor, filesVersionsRegistry);
            var indexUpdater = new IndexUpdater(indexer, filesVersionsRegistry);
            var fileWatcherFactory = new FileSystemWatcherFactory(
                indexer,
                indexUpdater,
                indexEjector,
                new FilePathActualizer(filesVersionsRegistry)
            );
            var fileSupervisor = new FileSupervisor(fileWatcherFactory);
            var searcher = new Searcher(index);
            var indexCleaner = new IndexCleaner(index, filesVersionsRegistry, settings);
            var scheduler = new Scheduler(indexCleaner, settings);
            return new SearchEngine(eventReactor, indexer, indexEjector, fileSupervisor, searcher, scheduler);
        }
    }
}