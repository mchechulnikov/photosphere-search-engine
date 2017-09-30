using System.IO;

namespace Jbta.DemoApp.ViewModels.IndexManagement.TreeView
{
    internal class FolderTreeViewItemViewModel : TreeViewItemViewModelBase
    {        
        private FolderTreeViewItemViewModel(string path, ITreeViewItemViewModel parent)
            : base(System.IO.Path.GetFileName(path), path, parent, isExpandable: true) {}

        public FolderTreeViewItemViewModel(string path)
            : base(path, path, isExpandable: true) {}

        private string Path => Content;

        protected override void LoadChildren()
        {
            Children.Clear();

            var directory = new DirectoryInfo(Path);
            foreach (var subDirectory in directory.GetDirectories())
            {
                Children.Add(new FolderTreeViewItemViewModel(subDirectory.FullName, this));
            }
            foreach (var file in directory.GetFiles())
            {
                Children.Add(new FileTreeViewItemViewModel(file.FullName, this));
            }
        }
    }
}