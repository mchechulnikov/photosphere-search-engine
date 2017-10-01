namespace Jbta.SearchEngine.DemoApp.ViewModels.SearchPanel
{
    internal class ListBoxItemViewModel : ViewModelBase
    {
        private readonly string _path;
        private readonly int _position;
        private readonly int _lineNumber;

        public ListBoxItemViewModel(string path, int lineNumber, int position)
        {
            _path = path;
            _lineNumber = lineNumber;
            _position = position;
        }

        private string EntryPosition => $"({_lineNumber}:{_position})";

        public override string ToString() => $"{_path} {EntryPosition}";
        //public override string ToString() => $"{Path.GetFileName(_path)} {EntryPosition}";
    }
}