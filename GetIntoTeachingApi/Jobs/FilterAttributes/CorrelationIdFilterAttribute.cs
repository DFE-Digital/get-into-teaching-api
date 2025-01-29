using Hangfire.Common;
using System;

namespace GetIntoTeachingApi.Jobs.FilterAttributes
{
    /// <summary>
    /// Provides a passive attribute filter which allows us to inject
    /// the required <see cref="IHttpContextCorrelationIdProvider"/>
    /// dependency into the associated job (coupled by naming
    /// convention, i.e. 'CorrelationIdFilter').
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public sealed class CorrelationIdFilterAttribute : JobFilterAttribute{
    }
}
