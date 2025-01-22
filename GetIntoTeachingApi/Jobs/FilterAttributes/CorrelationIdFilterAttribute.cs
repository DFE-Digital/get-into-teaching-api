using Hangfire.Common;

namespace GetIntoTeachingApi.Jobs.FilterAttributes
{
    /// <summary>
    /// Provides a passive attribute filter which allows us to inject
    /// the required <see cref="IHttpContextCorrelationIdProvider"/>
    /// dependency into the associated job (coupled by naming
    /// convention, i.e. 'CorrelationIdFilter').
    /// </summary>
    public sealed class CorrelationIdFilterAttribute : JobFilterAttribute{
    }
}
