using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common.Extensions
{
    /// <summary>
    /// Extensions to support chaining evaluation handlers.
    /// </summary>
    public static class ChainEvaluationHandlerExtensions
    {
        /// <summary>
        /// Provides a convenient method for chaining
        /// lists of evaluation handlers in a single call.
        /// </summary>
        /// <typeparam name="TRequest">
        /// Defines the request type associated with the evaluation handler.
        /// </typeparam>
        /// <typeparam name="TResponse">
        /// Defines the response type associated with the evaluation handler.
        /// </typeparam>
        /// <param name="evaluationHandlers">
        /// The list of evaluation handlers to be chained.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the evaluation handlers are not provided.
        /// </exception>
        public static void ChainEvaluationHandlers<TRequest, TResponse>(
            this IList<IChainEvaluationHandler<TRequest, TResponse>> evaluationHandlers)
        {
            ArgumentNullException.ThrowIfNull(evaluationHandlers);

            for (int evaluationHandlerTally = 0;
                evaluationHandlerTally < evaluationHandlers.Count;
                evaluationHandlerTally++)
            {
                if (evaluationHandlerTally + 1 < evaluationHandlers.Count)
                {
                    evaluationHandlers[evaluationHandlerTally]
                        .ChainNextHandler(evaluationHandlers[evaluationHandlerTally + 1]);
                }
            }
        }
    }
}
