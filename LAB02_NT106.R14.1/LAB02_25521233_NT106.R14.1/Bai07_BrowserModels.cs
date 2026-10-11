#nullable disable

using System;
using System.IO;

namespace Lab02.Bai07Browser
{
    internal sealed class ExplorerItem
    {
        public ExplorerItem(string fullPath, bool isDirectory)
        {
            FullPath = fullPath;
            IsDirectory = isDirectory;
            string root = Path.GetPathRoot(fullPath);
            IsDriveRoot = isDirectory && string.Equals(root, fullPath, StringComparison.OrdinalIgnoreCase);
        }

        public string FullPath { get; }

        public bool IsDirectory { get; }

        public bool IsDriveRoot { get; }

        public bool IsLoaded { get; set; }

        public int ChildFolderCount { get; set; }

        public int ChildFileCount { get; set; }

        public string Name
        {
            get
            {
                if (IsDriveRoot)
                {
                    return FullPath;
                }

                string name = Path.GetFileName(FullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                return name.Length == 0 ? FullPath : name;
            }
        }
    }

    internal sealed class SearchHit
    {
        public string FullPath { get; set; }

        public string Name { get; set; }

        public string ParentPath { get; set; }

        public bool IsDirectory { get; set; }

        public long SizeBytes { get; set; }
    }

    internal sealed class FolderStatistics
    {
        public long TotalBytes { get; set; }

        public int FileCount { get; set; }

        public int FolderCount { get; set; }

        public int SkippedFolderCount { get; set; }
    }

    public sealed class FavoriteEntry
    {
        public string Path { get; set; }

        public string AddedAt { get; set; }
    }
}
