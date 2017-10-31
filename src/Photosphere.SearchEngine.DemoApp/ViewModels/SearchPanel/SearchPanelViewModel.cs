using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Photosphere.SearchEngine.DemoApp.Model;
using Photosphere.SearchEngine.DemoApp.Utils;
using Photosphere.SearchEngine.Events.Args;

namespace Photosphere.SearchEngine.DemoApp.ViewModels.SearchPanel
{
    internal class SearchPanelViewModel : ViewModelBase
    {
        private string _searchString = string.Empty;
        private bool _isWholeWord;
        private bool _onlyFilesSearch;

        public SearchPanelViewModel()
        {
            ListBoxItems = new ObservableCollection<ListBoxItemViewModel>();

            SubscribeOnIndexStateChange();
        }

        public string SearchText
        {
            get => _searchString;
            set
            {
                var val = value?.Trim();
                SetField(ref _searchString, val, nameof(SearchText));
                Search(val, OnlyFilesSearch, IsWholeWord);
            }
        }

        public bool IsWholeWord
        {
            get => _isWholeWord;
            set
            {
                SetField(ref _isWholeWord, value, nameof(IsWholeWord));
                Search(SearchText, OnlyFilesSearch, value);
            }
        }

        public bool OnlyFilesSearch
        {
            get => _onlyFilesSearch;
            set
            {
                SetField(ref _onlyFilesSearch, value, nameof(OnlyFilesSearch));
                Search(SearchText, value, IsWholeWord);
            }
        }

        public ObservableCollection<ListBoxItemViewModel> ListBoxItems { get; }

        private void SubscribeOnIndexStateChange()
        {
            SearchSystem.EngineInstance.FileIndexingStarted += OnIndexStateChange;
            SearchSystem.EngineInstance.FileIndexingEnded += OnIndexStateChange;
            SearchSystem.EngineInstance.FileRemovingStarted += OnIndexStateChange;
            SearchSystem.EngineInstance.FileRemovingEnded += OnIndexStateChange;
            SearchSystem.EngineInstance.FilePathChanged += OnIndexStateChange;

            void OnIndexStateChange(SearchEngineEventArgs a)
            {
                DispatchService.Invoke(() => Search(_searchString, _onlyFilesSearch, _isWholeWord));
            }
        }

        private void Search(string value, bool onlyFilesSearch, bool isWholeWord)
        {
            ListBoxItems.Clear();
            if (value.Length < 3)
            {
                return;
            }

            if (onlyFilesSearch)
            {
                var serchResult = SearchSystem.EngineInstance.SearchFiles(value, isWholeWord);
                if (serchResult == null)
                {
                    return;
                }

                AddResultsToList(serchResult);
            }
            else
            {
                var serchResult = SearchSystem.EngineInstance.Search(value, isWholeWord);
                if (serchResult == null)
                {
                    return;
                }

                AddResultsToList(serchResult);
            }
        }

        private void AddResultsToList(IEnumerable<string> result)
        {
            var orderedResult = result
                .OrderBy(r => r)
                .Take(500)
                .ToList();

            foreach (var path in orderedResult)
            {
                var item = new ListBoxItemViewModel(path);
                ListBoxItems.Add(item);
            }
        }

        private void AddResultsToList(IEnumerable<WordEntry> result)
        {
            var orderedResult = result
                .OrderBy(r => r.FileVersion.Path)
                .ThenBy(r => r.LineNumber)
                .ThenBy(r => r.Position)
                .Take(500)
                .ToList();

            foreach (var wordEntry in orderedResult)
            {
                var item = new ListBoxItemViewModel(wordEntry.FileVersion.Path, wordEntry.LineNumber, wordEntry.Position);
                ListBoxItems.Add(item);
            }
        }
    }
}
