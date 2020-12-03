using Microsoft.AspNetCore.Mvc;

namespace GetIntoTeachingApi.Attributes
{
    public class PrivateShortTermResponseCacheAttribute : ResponseCacheAttribute
    {
        public PrivateShortTermResponseCacheAttribute()
        {
            Location = ResponseCacheLocation.Client;
            Duration = 300;
        }
    }
}