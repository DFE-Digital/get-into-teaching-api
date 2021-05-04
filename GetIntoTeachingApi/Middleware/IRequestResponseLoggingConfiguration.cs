using System.Text.RegularExpressions;

namespace GetIntoTeachingApi.Middleware
{
    public interface IRequestResponseLoggingConfiguration
    {
        public Regex[] CompactLoggingPatterns { get; }
    }
}
