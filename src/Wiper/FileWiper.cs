using System;
using System.IO;

namespace Wiper
{
    public class FileWiper
    {
        private const long bytesToDelete = 20;
        private static byte[] _bytes = new byte[bytesToDelete];
        
        public static bool WipeFile(string filePath)
        {
            Console.WriteLine($"Wiping file {filePath}");
            long length = new FileInfo(filePath).Length;
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                stream.Position = 0;
                stream.Write(_bytes, 0, (int)Math.Min(bytesToDelete, length) - 1);
                stream.Flush();
                stream.Close();
            }

            return true;
        }
    }
}