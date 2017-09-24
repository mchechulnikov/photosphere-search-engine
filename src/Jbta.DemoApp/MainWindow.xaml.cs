using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Jbta.DemoApp
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnAddFolderButtonClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    OnFolderSelected(dialog.SelectedPath);
                }
            }
        }

        private void OnFolderSelected(string folderPath)
        {
            var directory = new DirectoryInfo(folderPath);
            FilesTree.Items.Add(CreateDirectoryNode(directory, true));
        }

        private void OnAddFilesButtonClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                dialog.Multiselect = true;
                dialog.DefaultExt = ".txt";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = "Text files (*.txt)|*.txt|Log Files (*.log)|*.log|C# Files (*.cs)|*.cs";

                var dialogResult = dialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK && dialog.FileNames != null &&
                    dialog.FileNames.Any())
                {
                    OnFileSelected(dialog.FileNames);
                }
            }
        }

        private void OnFileSelected(IEnumerable<string> pathes)
        {
            AddFilesButton.IsEnabled = false;
            AddFolderButton.IsEnabled = false;
            RemoveButton.IsEnabled = false;
            FilesTree.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
            for (var i = 0; i < 100; i++)
            {
                // Progress bar imitation
                var i1 = i;
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = i1, DispatcherPriority.Background);
                Thread.Sleep(3);
            }
            ProgressBar.Value = 0;
            ProgressBar.Visibility = Visibility.Hidden;
            AddFilesButton.IsEnabled = true;
            AddFolderButton.IsEnabled = true;
            RemoveButton.IsEnabled = true;
            FilesTree.IsEnabled = true;

            foreach (var path in pathes)
            {
                var file = new FileInfo(path);
                FilesTree.Items.Add(new TreeViewItem { Header = file.FullName });
            }
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo, bool includePath = false)
        {
            var directoryNode = new TreeViewItem
            {
                Header = includePath ? directoryInfo.FullName : directoryInfo.Name,
                Tag = directoryInfo,
            };
            if (directoryInfo.EnumerateFileSystemInfos().Any())
            {
                directoryNode.Items.Add("Loading...");
            }
            return directoryNode;
        }

        private void OnFolderExpanded(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is TreeViewItem expandedItem) || expandedItem.Items.Count != 1 || !(expandedItem.Items[0] is string))
            {
                return;
            }
            expandedItem.Items.Clear();

            DirectoryInfo expandedDirectory;
            if (expandedItem.Tag is DriveInfo)
            {
                expandedDirectory = ((DriveInfo)expandedItem.Tag).RootDirectory;
            }
            else if (expandedItem.Tag is DirectoryInfo)
            {
                expandedDirectory = (DirectoryInfo)expandedItem.Tag;
            }
            else
            {
                return;
            }

            foreach (var subDirectory in expandedDirectory.GetDirectories())
            {
                var item = new TreeViewItem
                {
                    Header = subDirectory.ToString(),
                    Tag = subDirectory
                };
                item.Items.Add("Loading...");
                expandedItem.Items.Add(item);
            }
            foreach (var file in expandedDirectory.GetFiles())
            {
                expandedItem.Items.Add(new TreeViewItem { Header = file.Name });
            }
        }

        private void OnRemoveButtonClick(object sender, RoutedEventArgs e)
        {
            // todo
            foreach (TreeViewItem item in FilesTree.Items)
            {
                if (item.IsSelected)
                {
                    FilesTree.Items.Remove(item);
                    return;
                }
                foreach (var subItem in item.Items)
                {
                    if (item.IsSelected)
                    {
                        item.Items.Remove(subItem);
                        return;
                    }
                }
            }
        }
    }
}