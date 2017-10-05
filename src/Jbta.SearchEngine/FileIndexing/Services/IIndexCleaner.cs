namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal interface ICleaner
    {
        bool IsBusy { get; }
        void CleanUp();
    }
}