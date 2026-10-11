#nullable disable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Lab02.Bai07Browser
{
    internal static class TextFolding
    {
        public static string Fold(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            bool isPlainAscii = true;
            foreach (char letter in text)
            {
                if (letter > 127)
                {
                    isPlainAscii = false;
                    break;
                }
            }

            if (isPlainAscii)
            {
                return text.ToLowerInvariant();
            }

            string decomposed = text.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder(decomposed.Length);
            foreach (char letter in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(letter);
                }
            }

            return builder.ToString().ToLowerInvariant().Replace('đ', 'd');
        }
    }

    internal static class FolderScanner
    {
        private const int ProgressInterval = 400;
        private const int HashBufferBytes = 1024 * 1024;

        public static List<SearchHit> SearchByName(string rootPath, string keyword, int maxResults, CancellationToken token, IProgress<string> progress)
        {
            string foldedKeyword = TextFolding.Fold(keyword);
            List<SearchHit> hits = new List<SearchHit>();
            Stack<string> pendingFolders = new Stack<string>();
            pendingFolders.Push(rootPath);
            int visitedEntries = 0;

            while (pendingFolders.Count > 0 && hits.Count < maxResults)
            {
                token.ThrowIfCancellationRequested();
                string folderPath = pendingFolders.Pop();

                try
                {
                    foreach (FileSystemInfo entry in new DirectoryInfo(folderPath).EnumerateFileSystemInfos())
                    {
                        visitedEntries++;
                        if (visitedEntries % ProgressInterval == 0)
                        {
                            token.ThrowIfCancellationRequested();
                            progress?.Report("Đang quét: " + folderPath);
                        }

                        bool isDirectory = (entry.Attributes & FileAttributes.Directory) != 0;
                        if (TextFolding.Fold(entry.Name).Contains(foldedKeyword))
                        {
                            hits.Add(new SearchHit
                            {
                                FullPath = entry.FullName,
                                Name = entry.Name,
                                ParentPath = folderPath,
                                IsDirectory = isDirectory,
                                SizeBytes = isDirectory ? 0 : ((FileInfo)entry).Length
                            });

                            if (hits.Count >= maxResults)
                            {
                                break;
                            }
                        }

                        if (isDirectory && (entry.Attributes & FileAttributes.ReparsePoint) == 0)
                        {
                            pendingFolders.Push(entry.FullName);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
            }

            return hits;
        }

        public static FolderStatistics MeasureFolder(string rootPath, CancellationToken token, IProgress<string> progress)
        {
            FolderStatistics statistics = new FolderStatistics();
            Stack<string> pendingFolders = new Stack<string>();
            pendingFolders.Push(rootPath);
            int visitedEntries = 0;

            while (pendingFolders.Count > 0)
            {
                token.ThrowIfCancellationRequested();
                string folderPath = pendingFolders.Pop();

                try
                {
                    foreach (FileSystemInfo entry in new DirectoryInfo(folderPath).EnumerateFileSystemInfos())
                    {
                        visitedEntries++;
                        if (visitedEntries % ProgressInterval == 0)
                        {
                            token.ThrowIfCancellationRequested();
                            progress?.Report("Đang đo: " + statistics.FileCount.ToString("N0") + " tệp · " + statistics.FolderCount.ToString("N0") + " thư mục");
                        }

                        if ((entry.Attributes & FileAttributes.Directory) != 0)
                        {
                            if ((entry.Attributes & FileAttributes.ReparsePoint) == 0)
                            {
                                statistics.FolderCount++;
                                pendingFolders.Push(entry.FullName);
                            }
                        }
                        else
                        {
                            statistics.FileCount++;
                            statistics.TotalBytes += ((FileInfo)entry).Length;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    statistics.SkippedFolderCount++;
                }
                catch (IOException)
                {
                    statistics.SkippedFolderCount++;
                }
            }

            return statistics;
        }

        public static string ComputeSha256(string filePath, CancellationToken token, IProgress<int> percentProgress)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                byte[] buffer = new byte[HashBufferBytes];
                long totalBytes = Math.Max(1L, stream.Length);
                long readBytes = 0;
                int lastPercent = -1;
                int count;

                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    token.ThrowIfCancellationRequested();
                    hash.AppendData(buffer, 0, count);
                    readBytes += count;

                    int percent = (int)(readBytes * 100 / totalBytes);
                    if (percent != lastPercent)
                    {
                        lastPercent = percent;
                        percentProgress?.Report(percent);
                    }
                }

                return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
            }
        }
    }
}
