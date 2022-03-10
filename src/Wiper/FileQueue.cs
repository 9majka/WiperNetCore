using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Wiper
{
    public class FileQueue
    {
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private readonly object _monitor = new object();
        private bool _queueFillCompleted = false;
        
        public void PutFile(string filePath)
        {
            lock (_monitor)
            {
                _queue.Enqueue(filePath);
                Monitor.PulseAll(_monitor);
            }
        }

        public void NotifyLast()
        {
            _queueFillCompleted = true;
        }

        public bool Empty()
        {
            return _queue.Count == 0;
        }
        
        public bool FillInProgress()
        {
            return !_queueFillCompleted;
        }

        public string GetFilePath()
        {
            lock (_monitor)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_monitor);
                }
                
                if(_queue.TryDequeue(out string result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}