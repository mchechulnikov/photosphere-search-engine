using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Jbta.DemoApp.Model;

namespace Jbta.DemoApp.ViewModels.SearchPanel
{
    internal class SearchPanelViewModel : ViewModelBase
    {
        private string _searchString = string.Empty;

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

                ListBoxItems.Clear();
                if (value.Length < 3)
                {
                    return;
                }
                var serchResult = Index.Instance.Search(value);
                if (serchResult == null)
                {
                    return;
                }

                foreach (var wordEntry in serchResult.OrderBy(r => r.FileName).ThenBy(r => r.LineNumber).ThenBy(r => r.Position))
                {
                    var item = new ListBoxItemViewModel(wordEntry.FileName, wordEntry.LineNumber, wordEntry.Position);
                    ListBoxItems.Add(item);
                }
            }
        }

        public ObservableCollection<ListBoxItemViewModel> ListBoxItems { get; }
    }
}