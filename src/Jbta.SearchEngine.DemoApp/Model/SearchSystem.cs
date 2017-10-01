namespace Jbta.SearchEngine.DemoApp.Model
{
    public static class SearchSystem
    {
        public static readonly ISearchEngine EngineInstance = new WordsSearchEngine();
    }
}