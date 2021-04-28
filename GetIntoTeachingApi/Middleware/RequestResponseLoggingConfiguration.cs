using System.Text.RegularExpressions;

namespace GetIntoTeachingApi.Middleware
{
    public class RequestResponseLoggingConfiguration : IRequestResponseLoggingConfiguration
    {
        public Regex[] CompactLoggingPatterns
        {
            get
            {
                var options = RegexOptions.Compiled | RegexOptions.IgnoreCase;

                return new Regex[]
                {
                    new Regex(@"^GET /api/callback_booking_quotas", options),
                    new Regex(@"^GET /api/lookup_items", options),
                    new Regex(@"^GET /api/pick_list_items", options),
                    new Regex(@"^GET /api/privacy_policies", options),
                    new Regex(@"^GET /api/teaching_event_buildings", options),
                    new Regex(@"^GET /api/teaching_events", options),
                };
            }
        }
    }
}
