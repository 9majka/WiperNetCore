using System.Threading;
using System.Threading.Tasks;

namespace Wiper
{
    public class FileProcessor
    {
        private FileQueue _queue;
        private Task _task;
        private bool _completed = false;
        private readonly object _processMonitor = new object();
        
        public FileProcessor(FileQueue queue)
        {
            _queue = queue;
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(() =>
            {
                while (!_queue.Empty() || (_queue.Empty() && _queue.FillInProgress()))
                {
                    string filePath = _queue.GetFilePath();
                    FileWiper.WipeFile(filePath);
                }
                
                lock (_processMonitor)
                {
                    _completed = true;
                    Monitor.Pulse(_processMonitor);
                }
            });
        }
        
        public void WaitForProcessingComplete()
        {
            lock (_processMonitor)
            {
                while (!_completed)
                {
                    Monitor.Wait(_processMonitor);
                }
            }
        }
    }
}