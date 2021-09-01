using FluentAssertions;
using GetIntoTeachingApi.Utils;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class ZipFileCheckerTests
    {
        [Fact]
        public void AssureNoBombs_WhenCompressionRatioIsSuspicious_ThrowsBombFoundException()
        {
            const string suspiciousZipFilePath = "./Fixtures/zip-bomb-fixtures/25kb-uncompressed_1kb-compressed.zip";
            var checker = new ZipFileChecker(maxCompressionRatio: 10);

            Action check = () => checker.AssureNoBombs(suspiciousZipFilePath);

            check.Should()
                .Throw<BombFoundException>()
                .WithMessage("Compression ratio of 25 exceeds the maximum allowed (10)");
        }

        [Fact]
        public void AssureNoBombs_WhenNumberOfEntriesIsSuspicious_ThrowsBombFoundException()
        {
            const string suspiciousZipFilePath = "./Fixtures/zip-bomb-fixtures/contains-two-files.zip";
            var checker = new ZipFileChecker(maxNumberOfEntries: 1);

            Action check = () => checker.AssureNoBombs(suspiciousZipFilePath);

            check.Should()
                .Throw<BombFoundException>()
                .WithMessage("Found 2 entries which exceeds the maximum allowed (1)");
        }

        [Fact]
        public void AssureNoBombs_WhenArchiveSizeIsSuspicious_ThrowsBombFoundException()
        {
            const int oneKb = 1000;
            const string suspiciousZipFilePath = "./Fixtures/zip-bomb-fixtures/25kb-uncompressed_1kb-compressed.zip";
            var checker = new ZipFileChecker(maxTotalArchiveSize: oneKb, maxCompressionRatio: 1000);

            Action check = () => checker.AssureNoBombs(suspiciousZipFilePath);

            check.Should()
                .Throw<BombFoundException>()
                .WithMessage("The total archive size has exceeded the maximum allowed (1000 bytes)");
        }

        [Fact]
        public void AssureNoBombs_WhenZipIsNotSuspicious_DoesNotThrowBombFoundException()
        {
            const int thirtyKb = 30000;
            const string zipFilePath = "./Fixtures/zip-bomb-fixtures/25kb-uncompressed_1kb-compressed.zip";
            var checker = new ZipFileChecker(maxTotalArchiveSize: thirtyKb, maxCompressionRatio: 1000);

            Action check = () => checker.AssureNoBombs(zipFilePath);

            check.Should().NotThrow<BombFoundException>();
        }
    }
}
