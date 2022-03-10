using System;

namespace Wiper
{
    public class FileWiper
    {
        public static bool WipeFile(string filePath)
        {
            Console.WriteLine($"Wiping file {filePath}");
            return true;
        }
    }
}