#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Lab02.Bai07Browser
{
    internal sealed class FavoriteStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

        private readonly List<FavoriteEntry> entries = new List<FavoriteEntry>();
        private readonly string storageFilePath;

        public FavoriteStore()
        {
            string directory = System.IO.Path.Combine(AppContext.BaseDirectory, "Data");
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
            }

            storageFilePath = System.IO.Path.Combine(directory, "Bai07_YeuThich.json");
            LoadFromDisk();
        }

        public IReadOnlyList<FavoriteEntry> Entries
        {
            get { return entries; }
        }

        public bool Contains(string path)
        {
            return entries.Exists(entry => IsSamePath(entry.Path, path));
        }

        public bool Toggle(string path)
        {
            int index = entries.FindIndex(entry => IsSamePath(entry.Path, path));
            bool nowFavorite = index < 0;
            if (nowFavorite)
            {
                entries.Add(new FavoriteEntry { Path = path, AddedAt = DateTime.Now.ToString("s") });
            }
            else
            {
                entries.RemoveAt(index);
            }

            SaveToDisk();
            return nowFavorite;
        }

        public void Remove(string path)
        {
            entries.RemoveAll(entry => IsSamePath(entry.Path, path));
            SaveToDisk();
        }

        private static bool IsSamePath(string first, string second)
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        private void LoadFromDisk()
        {
            try
            {
                if (!File.Exists(storageFilePath))
                {
                    return;
                }

                List<FavoriteEntry> loaded = JsonSerializer.Deserialize<List<FavoriteEntry>>(File.ReadAllText(storageFilePath));
                if (loaded != null)
                {
                    entries.AddRange(loaded);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is JsonException)
            {
            }
        }

        private void SaveToDisk()
        {
            try
            {
                File.WriteAllText(storageFilePath, JsonSerializer.Serialize(entries, JsonOptions));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
            }
        }
    }
}
