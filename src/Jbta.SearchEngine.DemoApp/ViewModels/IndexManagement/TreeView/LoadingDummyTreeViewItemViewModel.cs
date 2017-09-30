namespace Jbta.DemoApp.ViewModels.IndexManagement.TreeView
{
    internal sealed class LoadingDummyTreeViewItemViewModel : TreeViewItemViewModelBase
    {
        public static readonly ITreeViewItemViewModel Instance =
            new LoadingDummyTreeViewItemViewModel("Loading...");

        private LoadingDummyTreeViewItemViewModel(string name) : base(name) {}
    }
}