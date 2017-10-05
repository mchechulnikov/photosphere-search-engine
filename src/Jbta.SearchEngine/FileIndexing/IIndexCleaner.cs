namespace Jbta.SearchEngine.FileIndexing
{
    internal interface ICleaner
    {
        bool IsBusy { get; }
        void CleanUp();
    }
}