using System.Threading.Tasks;

namespace Wiper
{
    public class Scanner
    {
        private FileQueue _queue;
        private Task _task;
        public Scanner(FileQueue queue)
        {
            _queue = queue;
        }

        public void Start(string rootPath)
        {
            int i = 0;
            _task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    _queue.PutFile($"{rootPath}/File{i}");
                    i++;
                    if(i > 20) return;
                }

            });
        }
    }
}