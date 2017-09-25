using System.Collections.ObjectModel;
using System.Linq;

namespace Jbta.DemoApp.ViewModels.IndexManagement.TreeView
{
    internal abstract class TreeViewItemViewModelBase : ViewModelBase, ITreeViewItemViewModel
    {
        private bool _isExpanded;
        private bool _isSelected;

        protected TreeViewItemViewModelBase(string name)
        {
            Name = name;
        }

        protected TreeViewItemViewModelBase(string name, string content, bool isExpandable)
        {
            Name = name;
            Content = content;

            if (!isExpandable)
            {
                return;
            }

            Children = new ObservableCollection<ITreeViewItemViewModel>
            {
                LoadingDummyTreeViewItemViewModel.Instance
            };
        }

        protected TreeViewItemViewModelBase(string name, string content, ITreeViewItemViewModel parent, bool isExpandable)
        {
            Name = name;
            Content = content;
            Parent = parent;
            if (!isExpandable)
            {
                return;
            }

            Children = new ObservableCollection<ITreeViewItemViewModel>
            {
                LoadingDummyTreeViewItemViewModel.Instance
            };
            
        }

        public string Name { get; }

        public string Content { get; }

        public ObservableCollection<ITreeViewItemViewModel> Children { get; }

        public ITreeViewItemViewModel Parent { get; }

        public bool IsChildrenLoaded =>
            Children == null || Children.Count != 1 || Children.FirstOrDefault() != LoadingDummyTreeViewItemViewModel.Instance;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }

                if (_isExpanded && Parent != null)
                {
                    Parent.IsExpanded = true;
                }

                // Lazy load the child items, if necessary.
                if (IsChildrenLoaded)
                {
                    return;
                }

                Children.Remove(LoadingDummyTreeViewItemViewModel.Instance);
                LoadChildren();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected)
                {
                    return;
                }

                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        protected virtual void LoadChildren() {}
    }
}