using System.Collections.ObjectModel;

namespace Photosphere.SearchEngine.DemoApp.ViewModels.IndexManagement.TreeView
{
    public interface ITreeViewItemViewModel
    {
        string Name { get; }
        string Content { get; }
        ObservableCollection<ITreeViewItemViewModel> Children { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
    }
}