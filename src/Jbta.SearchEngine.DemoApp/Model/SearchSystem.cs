namespace Jbta.SearchEngine.DemoApp.Model
{
    public static class SearchSystem
    {
        public static readonly ISearchEngine Instance = new WordsSearchEngine();
    }
}