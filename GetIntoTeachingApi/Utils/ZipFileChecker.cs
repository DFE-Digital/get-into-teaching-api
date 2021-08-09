using System.IO;
using System.IO.Compression;

namespace GetIntoTeachingApi.Utils
{
    public static class ZipFileChecker
    {
        private const int MaxNumberOfEntries = 10000;
        private const int MaxTotalArchiveSize = 1000000000; // 1GB
        private const int MaxCompressionRatio = 10;

        public static void AssureNoBombs(string zipPath)
        {
            using var zipFile = ZipFile.OpenRead(zipPath);

            CheckNumberOfEntries(zipFile);

            CheckSizeOfArchives(zipFile);
        }

        private static void CheckSizeOfArchives(ZipArchive zipFile)
        {
            int totalArchiveSize = 0;

            foreach (ZipArchiveEntry entry in zipFile.Entries)
            {
                using (Stream st = entry.Open())
                {
                    byte[] buffer = new byte[1024];
                    int totalEntrySize = 0;
                    int numberOfBytesRead = 0;

                    do
                    {
                        numberOfBytesRead = st.Read(buffer, 0, 1024);
                        totalEntrySize += numberOfBytesRead;
                        totalArchiveSize += numberOfBytesRead;
                        double compressionRatio = totalEntrySize / (double)entry.CompressedLength;

                        if (compressionRatio > MaxCompressionRatio)
                        {
                            throw new BombFoundException(
                                $"Compression ratio of ${compressionRatio} exceeds the maximum allowed ${MaxCompressionRatio}");
                        }
                    }
                    while (numberOfBytesRead > 0);
                }

                if (totalArchiveSize > MaxTotalArchiveSize)
                {
                    throw new BombFoundException(
                        $"Total archive size ${totalArchiveSize} exceeds the maximum allowed ${MaxTotalArchiveSize}");
                }
            }
        }

        private static void CheckNumberOfEntries(ZipArchive zipArchive)
        {
            int numberOfEntries = zipArchive.Entries.Count;
            if (numberOfEntries > MaxNumberOfEntries)
            {
                throw new BombFoundException(
                    $"Found ${zipArchive.Entries.Count} entries which exceeds the maximum allowed ${MaxNumberOfEntries}");
            }
        }
    }
}