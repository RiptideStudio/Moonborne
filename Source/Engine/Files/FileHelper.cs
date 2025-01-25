using System.Collections.Generic;
using System;
using System.IO;

namespace Moonborne.Engine.FileSystem
{
    public static class FileHelper
    {
        public static void DeleteFile(string path)
        {
            // Delete the file
            if (File.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine($"Deleted {path}");
            }
        }
    }
}
