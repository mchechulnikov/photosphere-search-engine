using System.Collections.ObjectModel;

namespace Jbta.DemoApp.ViewModels.IndexManagement.TreeView
{
    public interface ITreeViewItemViewModel
    {
        string Name { get; }
        string Content { get; }
        ITreeViewItemViewModel Parent { get; }
        ObservableCollection<ITreeViewItemViewModel> Children { get; }
        bool IsChildrenLoaded { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
    }
}