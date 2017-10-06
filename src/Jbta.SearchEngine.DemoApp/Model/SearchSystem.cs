namespace Jbta.SearchEngine.DemoApp.Model
{
    internal static class SearchSystem
    {
        public static readonly ISearchEngine EngineInstance =
            SearchEngineFactory.New(new SearchEngineSettings { GcCollect = true });
    }
}