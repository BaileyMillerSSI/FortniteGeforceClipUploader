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
using DataAccess;

namespace WpfUploaderClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileWatcher _Watcher;
        private SqlServerLocalDB _Database;

        public MainWindow()
        {
            InitializeComponent();

            _Database = new SqlServerLocalDB(SqlServerLocalDB.GetOrCreateDatabaseFile());

            _Watcher = new FileWatcher(_Database);
            
            _Watcher.OnFileAdded += FileListUpdated;
            _Watcher.OnFileRemoved += FileListUpdated;

            _Watcher.StartWatching();

            VideoPreview.MediaEnded += MediaEnded;
        }

        private void MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoPreview.Stop();
            // Reset to preview mode
            VideoPreview.Position = TimeSpan.FromMilliseconds(0);
        }

        private async void FileListUpdated(object sender, FileInfo File)
        {



            var list = _Watcher.GetAllFiles();
            // Refresh List of Files
            Debug.WriteLine($"{list.Count()} files currently in watch.");


            await Dispatcher.InvokeAsync(()=> 
            {
                VideoList.Children.Clear();

                if (VideoPreview.Source != null && VideoPreview.Source.OriginalString == File.FullName)
                {
                    VideoPreview.Close();
                    VideoPreview.Visibility = Visibility.Collapsed;
                }

                foreach (var video in list)
                {
                    var btn = new Button()
                    {
                        Content = $"{video.Name}",
                        Padding = new Thickness(0, 25, 0, 0),
                        Tag = video
                    };

                    btn.Click += VideoPreviewLaunch;

                    // Loop and add buttons for the scroll view
                    VideoList.Children.Add(btn);
                }
            });

            
        }

        private async void VideoPreviewLaunch(object sender, RoutedEventArgs e)
        {
            var getVideoInfo = ((sender as Button).Tag as FileInfo);
            await Dispatcher.InvokeAsync(()=> 
            {
                VideoPreview.Visibility = Visibility.Visible;
                VideoPreview.Source = new Uri(getVideoInfo.FullName);
                VideoPreview.Play();
            });
        }

        private void Manual_Upload_Clicked(object sender, RoutedEventArgs e)
        {
            var list = _Watcher.GetAllFiles();
            // Refresh List of Files
            Debug.WriteLine($"{list.Count()} files currently in watch.");
        }
    }
}
