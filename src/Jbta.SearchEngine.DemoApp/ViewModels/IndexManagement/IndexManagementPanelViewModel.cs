using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Jbta.SearchEngine.DemoApp.Model;
using Jbta.SearchEngine.DemoApp.Utils;
using Jbta.SearchEngine.DemoApp.ViewModels.IndexManagement.TreeView;
using Jbta.SearchEngine.Events.Args;

namespace Jbta.SearchEngine.DemoApp.ViewModels.IndexManagement
{
    internal class IndexManagementPanelViewModel : ViewModelBase
    {
        private int _processingFilesCount;
        private bool _isIndexing;
        private Visibility _indexingStatusLabelVisibility = Visibility.Hidden;
        private RelayCommand _addFolderCommand;
        private RelayCommand _addFilesCommand;
        private RelayCommand _removeButtonClickCommand;

        public IndexManagementPanelViewModel()
        {
            TreeViewItems = new ObservableCollection<ITreeViewItemViewModel>();
            SubscribeOnIndexStateChange();
        }

        public bool IsIndexing
        {
            get => _isIndexing;
            set => SetField(ref _isIndexing, value, nameof(IsIndexing));
        }

        public Visibility IndexingStatusLabelVisibility
        {
            get => _indexingStatusLabelVisibility;
            set => SetField(ref _indexingStatusLabelVisibility, value, nameof(IndexingStatusLabelVisibility));
        }

        public ObservableCollection<ITreeViewItemViewModel> TreeViewItems { get; }

        public ICommand AddFolderButtonClick =>
            _addFolderCommand ?? (_addFolderCommand = new RelayCommand(OnAddFolderButtonClick));

        public ICommand AddFilesButtonClick =>
            _addFilesCommand ?? (_addFilesCommand = new RelayCommand(OnAddFilesButtonClick));

        public ICommand RemoveButtonClick =>
            _removeButtonClickCommand ?? (_removeButtonClickCommand = new RelayCommand(OnRemoveButtonClick));

        private void SubscribeOnIndexStateChange()
        {
            SearchSystem.EngineInstance.FileIndexing += OnStartFileProcessing;
            SearchSystem.EngineInstance.FileIndexed += OnStopFileProcessing;
            SearchSystem.EngineInstance.FileRemoving += OnStartFileProcessing;
            SearchSystem.EngineInstance.FileRemoved += OnStopFileProcessing;
            SearchSystem.EngineInstance.FilePathChanged += a => DispatchService.Invoke(RefreshTree);

            void OnStartFileProcessing(SearchEngineEventArgs a) => DispatchService.Invoke(() =>
            {
                Interlocked.Increment(ref _processingFilesCount);
                if (_processingFilesCount >= 1)
                {
                    IndexingStatusLabelVisibility = Visibility.Visible;
                }
            });

            void OnStopFileProcessing(SearchEngineEventArgs a) => DispatchService.Invoke(() =>
            {
                Interlocked.Decrement(ref _processingFilesCount);
                if (_processingFilesCount < 1)
                {
                    IndexingStatusLabelVisibility = Visibility.Hidden;
                }
            });
        }

        private async Task OnAddFolderButtonClick(object sender)
        {
            string selectedPath = null;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    selectedPath = dialog.SelectedPath;
                }
            }

            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return;
            }

            var willBeAdded = await Task.Run(() => SearchSystem.EngineInstance.Add(selectedPath));
            if (willBeAdded)
            {
                TreeViewItems.Add(new FolderTreeViewItemViewModel(selectedPath));
            }
        }

        private async Task OnAddFilesButtonClick(object sender)
        {
            var selectedPathes = GetFilesFromDialog();

            if (selectedPathes == null || !selectedPathes.Any())
            {
                return;
            }

            await AyncIndexing(selectedPathes);
        }

        private static string[] GetFilesFromDialog()
        {
            string[] selectedPathes = null;
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                dialog.Multiselect = true;
                dialog.DefaultExt = ".txt";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = "Text files (*.txt)|*.txt|Log Files (*.log)|*.log|C# Files (*.cs)|*.cs|All files (*.*)|*.*";

                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK && dialog.FileNames != null && dialog.FileNames.Any())
                {
                    selectedPathes = dialog.FileNames;
                }
            }
            return selectedPathes;
        }

        private async Task AyncIndexing(IReadOnlyCollection<string> pathes)
        {
            var tasks = pathes.Select(async path =>
            {
                var willBeAdded = await Task.Run(() => SearchSystem.EngineInstance.Add(path));
                if (willBeAdded)
                {
                    TreeViewItems.Add(new FileTreeViewItemViewModel(path));
                }
            });
            await Task.WhenAll(tasks);
        }

        private async Task OnRemoveButtonClick(object sender)
        {
            async Task RemoveAction(ICollection<ITreeViewItemViewModel> items)
            {
                foreach (var item in items.ToList())
                {
                    if (item.IsSelected)
                    {
                        items.Remove(item);
                        RemoveFromSearchSystem(item);
                        return;
                    }
                    if (item.Children != null)
                    {
                        await RemoveAction(item.Children);
                    }
                }
            }

            await RemoveAction(TreeViewItems);
        }

        private static void RemoveFromSearchSystem(ITreeViewItemViewModel item)
        {
            Task.Run(() => SearchSystem.EngineInstance.Remove(item.Content));
        }

        private void RefreshTree()
        {
            var indexedPathes = SearchSystem.EngineInstance.PathesUnderIndex;
            TreeViewItems.Clear();
            foreach (var path in indexedPathes.OrderBy(p => p))
            {
                if (FileSystem.IsDirectory(path))
                {
                    TreeViewItems.Add(new FolderTreeViewItemViewModel(path));
                }
                else
                {
                    TreeViewItems.Add(new FileTreeViewItemViewModel(path));
                }
            }
        }
    }
}