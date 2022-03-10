using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Wiper
{
    public class FileQueue
    {
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private readonly object _monitor = new object();
        
        public void PutFile(string filePath)
        {
            lock (_monitor)
            {
                _queue.Enqueue(filePath);
                Monitor.PulseAll(_monitor);
            }
        }

        public string GetFilePath()
        {
            lock (_monitor)
            {
                string result;
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_monitor);
                }
                
                if(_queue.TryDequeue(out result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}