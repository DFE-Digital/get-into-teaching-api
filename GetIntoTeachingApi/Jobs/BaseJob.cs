using System;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs.FilterAttributes;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;

namespace GetIntoTeachingApi.Jobs
{
    public abstract class BaseJob
    {
        private static readonly TimeSpan _deduplicateTimeSpan = TimeSpan.FromSeconds(5);
        private readonly IRedisService _redis;
        protected IEnv Env { get; init; }

        protected BaseJob(IEnv env, IRedisService redis)
        {
            Env = env;
            _redis = redis;
        }

        protected bool IsLastAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            var currentAttempt = CurrentAttempt(context, adapter);
            return currentAttempt >= JobConfiguration.Attempts(Env);
        }

        protected bool IsFirstAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            return CurrentAttempt(context, adapter) == 1;
        }

        protected int CurrentAttempt(PerformContext context, IPerformContextAdapter adapter)
        {
            return adapter.GetRetryCount(context) + 1;
        }

        protected string AttemptInfo(PerformContext context, IPerformContextAdapter adapter)
        {
            return $"{CurrentAttempt(context, adapter)}/{JobConfiguration.Attempts(Env)}";
        }

        protected bool Deduplicate(string signature, PerformContext context, IPerformContextAdapter adapter)
        {
            if (!IsFirstAttempt(context, adapter))
            {
                return false;
            }

            var redisKey = $"base_job.{GetType().Name}.{signature}";

            if (_redis.Database.KeyExists(redisKey))
            {
                return true;
            }
            else
            {
                _redis.Database.StringSet(redisKey, true, _deduplicateTimeSpan);
                return false;
            }
        }

        /// <summary>
        /// Gets teh correlation Id (content) from the given context (if available).
        /// </summary>
        /// <param name="context">
        /// The <see cref="PerformContext"/> instance provides
        /// the job parameters on which to extract (if available), the correlation Id.
        /// </param>
        /// <returns>
        /// The correlation Id (GUID) from the given context.
        /// </returns>
        protected Guid GetCorrelationId(PerformContext context) =>
            (context != null) ? context.GetJobParameter<Guid>(CorrelationIdFilter.CorrelationIdKey) : Guid.Empty;
    }
}
