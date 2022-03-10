using System.Threading.Tasks;

namespace Wiper
{
    public class FileProcessor
    {
        private FileQueue _queue;
        private Task _task;
        
        public FileProcessor(FileQueue queue)
        {
            _queue = queue;
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    string filePath = _queue.GetFilePath();
                    FileWiper.WipeFile(filePath);
                }
            });
        }
        
        
    }
}