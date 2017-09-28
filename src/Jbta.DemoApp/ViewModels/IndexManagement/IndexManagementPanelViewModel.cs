using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Jbta.DemoApp.Model;
using Jbta.DemoApp.Utils;
using Jbta.DemoApp.ViewModels.IndexManagement.TreeView;

namespace Jbta.DemoApp.ViewModels.IndexManagement
{
    internal class IndexManagementPanelViewModel : ViewModelBase
    {
        private bool _isIndexing;
        private Visibility _progressBarVisibility;
        private int _progressBarValue;
        private RelayCommand _addFolderCommand;
        private RelayCommand _addFilesCommand;
        private RelayCommand _removeButtonClickCommand;

        public IndexManagementPanelViewModel()
        {
            TreeViewItems = new ObservableCollection<ITreeViewItemViewModel>();
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

        public ObservableCollection<ITreeViewItemViewModel> TreeViewItems { get; }

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
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    selectedPath = dialog.SelectedPath;
                }
            }

            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return;
            }

            ProgressBarVisibility = Visibility.Visible;
            await Task.Run(() => Index.Instance.Add(selectedPath));
            ProgressBarValue += 100;
            TreeViewItems.Add(new FolderTreeViewItemViewModel(selectedPath));
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Hidden;
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
                if (dialogResult == DialogResult.OK && dialog.FileNames != null && dialog.FileNames.Any())
                {
                    selectedPathes = dialog.FileNames;
                }
            }

            if (selectedPathes == null || !selectedPathes.Any())
            {
                return;
            }

            ProgressBarVisibility = Visibility.Visible;
            await AyncIndexing(selectedPathes);
            ProgressBarVisibility = Visibility.Hidden;
            ProgressBarValue = 0;
        }

        private async Task AyncIndexing(string[] pathes)
        {
            var step = 100 / pathes.Length;
            var tasks = pathes.Select(async path =>
            {
                await Task.Run(() => Index.Instance.Add(path));
                ProgressBarValue += step;
                TreeViewItems.Add(new FileTreeViewItemViewModel(path));
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