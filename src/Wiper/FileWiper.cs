using System;
using System.IO;

namespace Wiper
{
    public class FileWiper
    {
        private const long bytesToDelete = 20 * 1024;
        private static byte[] _bytes = new byte[bytesToDelete];
        private static int sFilesWiped = 0;
        
        public static bool WipeFile(string filePath)
        {
            try
            {
                long length = new FileInfo(filePath).Length;
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    stream.Position = 0;
                    stream.Write(_bytes, 0, (int)Math.Min(bytesToDelete, length) );
                    stream.Flush();
                    stream.Close();
                }

                sFilesWiped++;

                if (sFilesWiped % 100 == 0)
                {
                    Console.WriteLine($"Files processed {sFilesWiped}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }


            return true;
        }
    }
}