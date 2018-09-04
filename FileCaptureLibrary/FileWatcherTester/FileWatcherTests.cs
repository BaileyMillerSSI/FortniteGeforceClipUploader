using FileCaptureLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileWatcherTester
{
    [TestClass]
    public class FileWatcherTests
    {
        [TestMethod]
        public void CanLocateFortniteFolder()
        {
            FileWatcher watcher = new FileWatcher();
            Assert.IsTrue(watcher.WatchedFolder.FullName == @"C:\Users\Bailey Miller\Videos\Fortnite");
            watcher.Dispose();
        }

        [TestMethod]
        public void CapturesFileEvents()
        {
            using (var watcher = new FileWatcher())
            {
                watcher.StartWatching();

                var dir = watcher.WatchedFolder;

                File.Delete(Path.Combine(dir.FullName, "testVideo.mp4"));
                File.Delete(Path.Combine(dir.FullName, "DeleteMe-testVideo.mp4"));

                var fileRef = File.Create(Path.Combine(dir.FullName, "testVideo.mp4"));
                fileRef.Close();
                var fileInfo = new FileInfo(fileRef.Name);

                // Pause for a second to ensure watcher catches this
                Thread.Sleep(1000);

                Assert.IsTrue(watcher.CreatedFiles.Count != 0 && watcher.CreatedFiles.Last().Name == fileInfo.Name, "New Video File Found");

                var newFileName = Path.Combine(fileInfo.DirectoryName, string.Concat("DeleteMe-", fileInfo.Name));

                // Do rename
                File.Move(fileRef.Name, newFileName);

                // Pause for a second to ensure watcher catches this
                Thread.Sleep(500);
                Assert.IsTrue(watcher.RenamedFiles.Count != 0 && watcher.RenamedFiles.Last().FullName == newFileName, "Renamed file found");

                Thread.Sleep(500);
                Assert.IsTrue(watcher.CreatedFiles.Count == 0, "Renamed file was removed from Created Files");

                // Do some cleanup
                File.Delete(newFileName);
            }
            
        }
    }
}
