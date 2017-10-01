using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Jbta.SearchEngine.DemoApp.Model;

namespace Jbta.SearchEngine.DemoApp.ViewModels.SearchPanel
{
    internal class SearchPanelViewModel : ViewModelBase
    {
        private string _searchString = string.Empty;
        private bool _isWholeWord;

        public SearchPanelViewModel()
        {
            ListBoxItems = new ObservableCollection<ListBoxItemViewModel>();
        }

        public string SearchText
        {
            get => _searchString;
            set
            {
                SetField(ref _searchString, value, nameof(SearchText));
                Search(value, IsWholeWord);
            }
        }

        public bool IsWholeWord
        {
            get => _isWholeWord;
            set
            {
                SetField(ref _isWholeWord, value, nameof(IsWholeWord));
                Search(SearchText, value);
            }
        }

        public ObservableCollection<ListBoxItemViewModel> ListBoxItems { get; }

        private void Search(string value, bool isWholeWord)
        {
            ListBoxItems.Clear();
            if (value.Length < 1)
            {
                return;
            }
            var serchResult = SearchSystem.Instance.Search(value, isWholeWord);
            if (serchResult == null)
            {
                return;
            }

            AddResultsToList(serchResult);
        }

        private void AddResultsToList(IEnumerable<WordEntry> result)
        {
            var orderedResult = result
                .OrderBy(r => r.FileVersion.Path)
                .ThenBy(r => r.LineNumber)
                .ThenBy(r => r.Position)
                .Take(500);

            foreach (var wordEntry in orderedResult)
            {
                var item = new ListBoxItemViewModel(wordEntry.FileVersion.Path, wordEntry.LineNumber, wordEntry.Position);
                ListBoxItems.Add(item);
            }
        }
    }
}
