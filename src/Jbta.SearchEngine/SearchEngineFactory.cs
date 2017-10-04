using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileIndexing.Services;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileSupervision;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.FileVersioning.Services;
using Jbta.SearchEngine.Searching;

namespace Jbta.SearchEngine
{
    public static class SearchEngineFactory
    {
        public static ISearchEngine New()
        {
            var settings = new Settings();
            return New(settings);
        }

        public static ISearchEngine New(Settings settings)
        {
            var eventReactor = new EventReactor();
            var fileParserProvider = new FileParserProvider(settings);
            var filesVersionsRegistry = new FilesVersionsRegistry(eventReactor);
            var index = new Index();
            var indexer = new FileIndexer(eventReactor, fileParserProvider, index, filesVersionsRegistry, settings);
            var indexEjector = new IndexEjector(eventReactor, index, filesVersionsRegistry, settings);
            var indexUpdater = new IndexUpdater(indexer, index, filesVersionsRegistry);
            var fileWatcherFactory = new FileSystemWatcherFactory(
                indexer,
                indexUpdater,
                indexEjector,
                new FilePathActualizer(filesVersionsRegistry)
            );
            var fileSupervisor = new FileSupervisor(fileWatcherFactory);
            var searcher = new Searcher(index);
            return new SearchEngine(eventReactor, indexer, indexEjector, fileSupervisor, searcher);
        }
    }
}