using System;
using System.IO;
using System.IO.Compression;

namespace GetIntoTeachingApi.Utils
{
    public class ZipFileChecker
    {
        private readonly int _maxNumberOfEntries;
        private readonly int _maxTotalArchiveSize;
        private readonly int _maxCompressionRatio;

        public ZipFileChecker(
            int maxNumberOfEntries = 10000,
            int maxTotalArchiveSize = 1000000000, // 1GB
            int maxCompressionRatio = 10)
        {
            _maxNumberOfEntries = maxNumberOfEntries;
            _maxTotalArchiveSize = maxTotalArchiveSize;
            _maxCompressionRatio = maxCompressionRatio;
        }

        public void AssureNoBombs(string zipPath)
        {
            using var zipFile = ZipFile.OpenRead(zipPath);

            CheckNumberOfEntries(zipFile);

            CheckSizeOfArchives(zipFile);
        }

        private void CheckSizeOfArchives(ZipArchive zipFile)
        {
            int totalArchiveSize = 0;

            foreach (ZipArchiveEntry entry in zipFile.Entries)
            {
                using Stream stream = entry.Open();
                byte[] buffer = new byte[1024];
                int totalEntrySize = 0;
                int numberOfBytesRead = 0;

                do
                {
                    numberOfBytesRead = stream.Read(buffer, 0, 1024);
                    totalEntrySize += numberOfBytesRead;
                    totalArchiveSize += numberOfBytesRead;
                    double compressionRatio = totalEntrySize / (double)entry.CompressedLength;

                    if (compressionRatio > _maxCompressionRatio)
                    {
                        throw new BombFoundException(
                            $"Compression ratio of {Math.Ceiling(compressionRatio)} exceeds the maximum allowed ({_maxCompressionRatio})");
                    }

                    if (totalArchiveSize > _maxTotalArchiveSize)
                    {
                        throw new BombFoundException(
                            $"The total archive size has exceeded the maximum allowed ({_maxTotalArchiveSize} bytes)");
                    }
                }
                while (numberOfBytesRead > 0);
            }
        }

        private void CheckNumberOfEntries(ZipArchive zipArchive)
        {
            int numberOfEntries = zipArchive.Entries.Count;
            if (numberOfEntries > _maxNumberOfEntries)
            {
                throw new BombFoundException(
                    $"Found {zipArchive.Entries.Count} entries which exceeds the maximum allowed ({_maxNumberOfEntries})");
            }
        }
    }
}