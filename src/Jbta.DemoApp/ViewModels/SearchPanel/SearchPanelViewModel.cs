using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Jbta.DemoApp.Model;
using Jbta.Indexing;

namespace Jbta.DemoApp.ViewModels.SearchPanel
{
    internal class SearchPanelViewModel : ViewModelBase
    {
        private string _searchString = string.Empty;
        private bool _isCaseSensetive = true;
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
                Search(value, IsCaseSensetive, IsWholeWord);
            }
        }

        public bool IsCaseSensetive
        {
            get => _isCaseSensetive;
            set
            {
                SetField(ref _isCaseSensetive, value, nameof(IsCaseSensetive));
                Search(SearchText, value, IsWholeWord);
            }
        }

        public bool IsWholeWord
        {
            get => _isWholeWord;
            set
            {
                SetField(ref _isWholeWord, value, nameof(IsWholeWord));
                Search(SearchText, IsCaseSensetive, value);
            }
        }

        public ObservableCollection<ListBoxItemViewModel> ListBoxItems { get; }

        private void Search(string value, bool isCaseSensetive, bool isWholeWord)
        {
            ListBoxItems.Clear();
            if (value.Length < 3)
            {
                return;
            }
            var serchResult = Index.Instance.Search(value, isCaseSensetive, isWholeWord);
            if (serchResult == null)
            {
                return;
            }

            AddResultsToList(serchResult);
        }

        private void AddResultsToList(IEnumerable<WordEntry> result)
        {
            var orderedResult = result
                .OrderBy(r => r.FileName)
                .ThenBy(r => r.LineNumber)
                .ThenBy(r => r.Position);

            foreach (var wordEntry in orderedResult)
            {
                var item = new ListBoxItemViewModel(wordEntry.FileName, wordEntry.LineNumber, wordEntry.Position);
                ListBoxItems.Add(item);
            }
        }
    }
}
