using FileCaptureLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUploaderClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileWatcher _Watcher;

        public MainWindow()
        {
            InitializeComponent();

            _Watcher = new FileWatcher();

            _Watcher.OnFileAdded += FileListUpdated;
            _Watcher.OnFileRemoved += FileListUpdated;

            _Watcher.StartWatching();
        }

        private void FileListUpdated(object sender, FileInfo File)
        {
            var list = _Watcher.GetAllFiles();
            // Refresh List of Files
            Debug.WriteLine($"{list.Count()} files currently in watch.");
        }

        private void Manual_Upload_Clicked(object sender, RoutedEventArgs e)
        {
            var list = _Watcher.GetAllFiles();
            // Refresh List of Files
            Debug.WriteLine($"{list.Count()} files currently in watch.");
        }
    }
}
