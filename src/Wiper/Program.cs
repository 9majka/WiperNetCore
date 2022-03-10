using System;
using System.Collections.Generic;

namespace Wiper
{
    class Program
    {
        private const int DefaultThreadCount = 5;

        private static void PrintHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("Wiper.exe -p D:\\ -n 5");
            Console.WriteLine("Wiper.exe -p D:\\");
            Console.WriteLine("Where:");
            Console.WriteLine("-p - path to disk drive");
            Console.WriteLine($"-n - number of threads. Default  {DefaultThreadCount}");
        }

        private static string GetValue(string[] args, string key)
        {
            string result = null;
            int size = args.Length;
            var indexOf = Array.IndexOf(args, key);
            if (indexOf >= 0 && indexOf < size - 1)
            {
                result = args[indexOf + 1];
            }

            return result;
        }
        
        private static bool ParseArgs(string[] args, out string diskDrivePath, out int threadCount)
        {
            diskDrivePath = GetValue(args, "-p");
            threadCount = DefaultThreadCount;
            string threadNumber = GetValue(args, "-n");
            
            if (string.IsNullOrEmpty(diskDrivePath))
            {
                Console.WriteLine("Disk path is null or empty");
                return false;
            }

            if (string.IsNullOrEmpty(threadNumber))
            {
                int tCount = 0;
                if(int.TryParse(threadNumber, out tCount))
                {
                    if (tCount > 0)
                    {
                        threadCount = tCount;
                    }
                }
            }

            return true;
        }

        static void Main(string[] args)
        {
            if(!ParseArgs(args, out string diskDrivePath, out int threadCount))
            {
                PrintHelp();
                return;
            }

            FileQueue queue = new FileQueue();
            Scanner scanner = new Scanner(queue);
            scanner.Start(diskDrivePath);

            List<FileProcessor> list = new List<FileProcessor>();
            for (int i = 0; i < threadCount; i++)
            {
                FileProcessor processor = new FileProcessor(queue);
                processor.Start();
                list.Add(processor);
            }
            
            scanner.WaitForScanCompleted();
            Console.WriteLine("Scan completed. Waiting for wipe complete");
            
            foreach (FileProcessor fileProcessor in list)
            {
                fileProcessor.WaitForProcessingComplete();
            }
            
            Console.WriteLine("Completed. Exit");
        }
    }
}