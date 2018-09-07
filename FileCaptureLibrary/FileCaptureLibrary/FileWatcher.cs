using DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCaptureLibrary
{
    public delegate void FileAddedToList(object sender, FileInfo File);

    public class FileWatcher: IDisposable
    {
        public event FileAddedToList OnFileAdded;
        public event FileAddedToList OnFileRemoved;

        public List<FileInfo> CreatedFiles { get; private set; } = new List<FileInfo>();
        public List<FileInfo> RenamedFiles { get; private set; } = new List<FileInfo>();

        public readonly DirectoryInfo WatchedFolder;
        
        private readonly FileSystemWatcher _Watcher;

        private readonly SqlServerLocalDB _Database;

        public FileWatcher(SqlServerLocalDB DB)
        {

            _Database = DB;

            WatchedFolder = LocateFortniteFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));


            _Watcher = new FileSystemWatcher()
            {
                EnableRaisingEvents = false,
                Filter = "*.mp4",
                Path= WatchedFolder.FullName,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            _Watcher.Created += FileCreated;
            _Watcher.Renamed += FileRenamed;
            _Watcher.Deleted += FileDeleted;
        }

        ~FileWatcher()
        {
            Dispose();
        }
        
        public void StartWatching()
        {
            _Watcher.EnableRaisingEvents = true;
        }

        public IEnumerable<FileInfo> GetAllFiles()
        {
            var uniqueFiles = new List<FileInfo>();
            uniqueFiles.AddRange(CreatedFiles);
            uniqueFiles.AddRange(RenamedFiles);
            // Just in case gets a unique list of the files
            return uniqueFiles.Distinct();
        }

        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            // Do some cleanup if the file gets deleted before needing it
            CreatedFiles.RemoveAll(x => x.FullName == e.FullPath);
            RenamedFiles.RemoveAll(x => x.FullName == e.FullPath);

            OnFileRemoved?.Invoke(this, new FileInfo(e.FullPath));
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            // Remove from created files if was an originally created file
            CreatedFiles.RemoveAll(x=>x.FullName == e.OldFullPath);

            Debug.WriteLine($"File was created {e.Name}");
            RenamedFiles.Add(new FileInfo(e.FullPath));

            OnFileAdded?.Invoke(this, new FileInfo(e.FullPath));

            AddOrUpdateFileInformation((new FileInfo(e.FullPath)));
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"File was created {e.Name}");
            CreatedFiles.Add(new FileInfo(e.FullPath));

            OnFileAdded?.Invoke(this, new FileInfo(e.FullPath));

            AddOrUpdateFileInformation((new FileInfo(e.FullPath)));
        }

        private DirectoryInfo LocateFortniteFolder(DirectoryInfo BaseDirectory)
        {
            var FortniteFolder = BaseDirectory.EnumerateDirectories().Where(x=>x.Name == "Fortnite").FirstOrDefault();
            
            return FortniteFolder ?? BaseDirectory;
        }


        private void AddOrUpdateFileInformation(FileInfo File)
        {
            if (_Database != null)
            {
                // Call code on Database
            }
        }

        public void Dispose()
        {
            if (_Watcher != null)
            {
                _Watcher.Dispose();
            }
        }
    }
}
