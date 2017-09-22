using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Jbta.DemoApp
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            var choosefileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose files or folder",
                Multiselect = true,
                DefaultExt = ".txt",
                Filter = "Text files (*.txt)|*.txt|Log Files (*.log)|*.log|C# Files (*.cs)|*.cs"
            };
            var isDialogShowed = choosefileDialog.ShowDialog();
            if (isDialogShowed ?? false)
            {
                OnFileSelected(choosefileDialog.FileNames);
            }
        }

        private void OnFileSelected(string[] pathes)
        {
            //MessageBox.Show(string.Join("\n\n", choosefileDialog.FileNames));
            AddButton.IsEnabled = false;
            RemoveButton.IsEnabled = false;
            FilesTree.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
            for (var i = 0; i < 100; i++)
            {
                // Progress bar imitation
                var i1 = i;
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = i1, DispatcherPriority.Background);
                Thread.Sleep(20);
            }
            ProgressBar.Value = 0;
            ProgressBar.Visibility = Visibility.Hidden;
            AddButton.IsEnabled = true;
            RemoveButton.IsEnabled = true;
            FilesTree.IsEnabled = true;


            FilesTree.Items.Clear();
            foreach (var path in pathes)
            {
                var file = new FileInfo(path);
                FilesTree.Items.Add(new TreeViewItem { Header = file.Name });
                //var rootDirectoryInfo = new DirectoryInfo(filePath);
                //FilesTree.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
            }
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Items.Add(CreateDirectoryNode(directory));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                directoryNode.Items.Add(new TreeViewItem { Header = file.Name });
            }
            return directoryNode;
        }
    }
}
