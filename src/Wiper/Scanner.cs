using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wiper
{
    public class Scanner
    {
        private FileQueue _queue;
        private Task _task;
        private bool _completed = false;
        private readonly object _scanMonitor = new object();
        
        public Scanner(FileQueue queue)
        {
            _queue = queue;
        }

        public void Start(string rootPath)
        {
            _task = Task.Factory.StartNew(() =>
            {
                Scan(rootPath);
            });
        }
        
        private void Scan(string rootDir)
        {
            Queue<string> subDirsQueue = new Queue<string>();
            if (Directory.Exists(rootDir))
            {
                subDirsQueue.Enqueue(rootDir);
            }
            else
            {
                //Log.Debug($"Root directory {rootDir} doesn't exist");
            }

            while (subDirsQueue.Count > 0)
            {
                string currentDir = subDirsQueue.Dequeue();
                var subDirsFound = GetSubDirsAndMatchFiles(currentDir);

                foreach (var subDir in subDirsFound)
                {
                    subDirsQueue.Enqueue(subDir);
                }
            }
            
            _queue.NotifyLast();

            lock (_scanMonitor)
            {
                _completed = true;
                Monitor.Pulse(_scanMonitor);
            }
        }

        public void WaitForScanCompleted()
        {
            lock (_scanMonitor)
            {
                while (!_completed)
                {
                    Monitor.Wait(_scanMonitor);
                }
            }
        }
        
        private delegate void FilesFoundCallback(string[] children);
        
        private string[] GetSubDirsAndMatchFiles(string currentDir)
        {
            return GetSubDirs(currentDir, (string[] fileList) =>
            {
                foreach (var file in fileList)
                {
                    _queue.PutFile(file);
                }
            });
        }
        
        private string[] GetSubDirs(string rootDir, FilesFoundCallback cb)
        {
            string[] subDirs = { };
            try
            {
                string[] filesFound = Directory.GetFiles(rootDir).ToArray();

                cb(filesFound);
                
                subDirs = Directory.GetDirectories(rootDir)
                    //Filtration of the subdirectories from presence of junction folders
                    .Where(dir => (new DirectoryInfo(dir).Attributes & FileAttributes.ReparsePoint) == 0)
                    //Filtration of the subdirectories from presence of already trawled folders 
                    .ToArray();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Sc UnauthorizedAccessException {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sc Exception {ex.Message}");
            }
            
            return subDirs;
        }
    }
}