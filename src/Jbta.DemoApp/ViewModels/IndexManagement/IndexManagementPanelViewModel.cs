using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Jbta.DemoApp.Utils;
using Jbta.DemoApp.ViewModels.IndexManagement.TreeView;

namespace Jbta.DemoApp.ViewModels.IndexManagement
{
    internal class IndexManagementPanelViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ITreeViewItemViewModel> _treeViewItems;
        private bool _isIndexing;
        private Visibility _progressBarVisibility;
        private int _progressBarValue;
        private RelayCommand _addFolderCommand;
        private RelayCommand _addFilesCommand;
        private RelayCommand _removeButtonClickCommand;

        public IndexManagementPanelViewModel()
        {
            _treeViewItems = new ObservableCollection<ITreeViewItemViewModel>();
        }

        public bool IsIndexing
        {
            get => _isIndexing;
            set => SetField(ref _isIndexing, value, nameof(IsIndexing));
        }

        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set => SetField(ref _progressBarVisibility, value, nameof(ProgressBarVisibility));
        }

        public int ProgressBarValue
        {
            get => _progressBarValue;
            set => SetField(ref _progressBarValue, value, nameof(ProgressBarValue));
        }

        public ObservableCollection<ITreeViewItemViewModel> TreeViewItems => _treeViewItems;

        public ICommand AddFolderButtonClick =>
            _addFolderCommand ?? (_addFolderCommand = new RelayCommand(OnAddFolderButtonClick));

        public ICommand AddFilesButtonClick =>
            _addFilesCommand ?? (_addFilesCommand = new RelayCommand(OnAddFilesButtonClick));

        public ICommand RemoveButtonClick =>
            _removeButtonClickCommand ?? (_removeButtonClickCommand = new RelayCommand(OnRemoveButtonClick));

        private async Task OnAddFolderButtonClick(object sender)
        {
            string selectedPath = null;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    selectedPath = dialog.SelectedPath;
                }
            }

            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return;
            }

            ProgressBarVisibility = Visibility.Visible;
            await AyncIndexing(selectedPath);
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Hidden;
            _treeViewItems.Add(new FolderTreeViewItemViewModel(selectedPath));
        }

        private async Task OnAddFilesButtonClick(object sender)
        {
            string[] selectedPathes = null;
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                dialog.Multiselect = true;
                dialog.DefaultExt = ".txt";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = "Text files (*.txt)|*.txt|Log Files (*.log)|*.log|C# Files (*.cs)|*.cs";

                var dialogResult = dialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK && dialog.FileNames != null && dialog.FileNames.Any())
                {
                    selectedPathes = dialog.FileNames;
                }
            }

            if (selectedPathes == null || !selectedPathes.Any())
            {
                return;
            }

            await OnFileSelected(selectedPathes);
        }

        private async Task OnFileSelected(string[] pathes)
        {
            ProgressBarVisibility = Visibility.Visible;
            await AyncIndexing(pathes);
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Hidden;

            foreach (var path in pathes)
            {
                _treeViewItems.Add(new FileTreeViewItemViewModel(path));
            }
        }

        private async Task AyncIndexing(params string[] pathes)
        {
            for (var i = 0; i < 100; i++)
            {
                // TODO
                ProgressBarValue = i;
                await Task.Delay(3);
            }
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
    }
}