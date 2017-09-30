namespace Jbta.SearchEngine.DemoApp.ViewModels.IndexManagement.TreeView
{
    internal class FileTreeViewItemViewModel : TreeViewItemViewModelBase
    {
        public FileTreeViewItemViewModel(string path, ITreeViewItemViewModel parent)
            : base(System.IO.Path.GetFileName(path), path, parent, isExpandable: false) {}

        public FileTreeViewItemViewModel(string path)
            : base(path, path, isExpandable: false) {}
    }
}