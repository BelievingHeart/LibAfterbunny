using System;
using System.IO;
using Lib_Enums;

namespace Utils
{
    public class FileSystem
    {
        public static string SolutionDir => Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        public static void Backup(string src, string dst)
        {
            if (File.Exists(dst)) File.Delete(dst);
            File.Copy(src, dst);
        }

        public static void Recover(string src, string dst)
        {
            Backup(src, dst);
        }

        public static void RemoveAllThatOutdates(string directory, int outdatedDays, FileOrDirectory fileOrDirectory)
        {
            var dirs = fileOrDirectory == FileOrDirectory.Directory
                ? Directory.GetDirectories(directory)
                : Directory.GetFiles(directory);

            foreach (var f in dirs)
            {
                var dt = fileOrDirectory == FileOrDirectory.Directory
                    ? Directory.GetCreationTime(f)
                    : File.GetCreationTime(f);
                var ts = DateTime.Now - dt;
                if (ts.Days <= outdatedDays) continue;
                if (fileOrDirectory == FileOrDirectory.Directory)
                    Directory.Delete(f, true);
                else
                    File.Delete(f);
            }
        }
    }
}