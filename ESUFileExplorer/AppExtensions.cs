using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUFileExplorer
{
    public static class AppExtensions
    {
        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (Directory.Exists(tempDirectory))
            {
                return GetTemporaryDirectory();
            }

            Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }

        public static int FileCount(string path, string fileExt)
        {
            return Directory.EnumerateFileSystemEntries(path, "*" + fileExt).Count();
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
