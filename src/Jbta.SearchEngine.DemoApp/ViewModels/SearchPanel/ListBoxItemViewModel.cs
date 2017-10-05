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

        public ListBoxItemViewModel(string path)
        {
            _path = path;
        }

        private string EntryPosition =>
            _lineNumber == 0 && _position == 0
                ? string.Empty
                : $" ({_lineNumber}:{_position})";

        public override string ToString() => $"{_path}{EntryPosition}";
    }
}