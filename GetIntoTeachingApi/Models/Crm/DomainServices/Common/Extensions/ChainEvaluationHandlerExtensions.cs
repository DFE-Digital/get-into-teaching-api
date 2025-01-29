using GetIntoTeachingApi.Models.Crm.DomainServices.Common;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ChainEvaluationHandlerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="evaluationHandlers"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ChainEvaluationHandlers<TRequest, TResponse>(
            this IList<IChainEvaluationHandler<TRequest, TResponse>> evaluationHandlers)
        {
            if (evaluationHandlers == null)
            {
                throw new ArgumentNullException(nameof(evaluationHandlers));
            }

            for (int jsonParsingHandlerTally = 0;
                jsonParsingHandlerTally < evaluationHandlers.Count;
                jsonParsingHandlerTally++)
            {
                if (jsonParsingHandlerTally + 1 < evaluationHandlers.Count)
                {
                    evaluationHandlers[jsonParsingHandlerTally]
                        .ChainNextHandler(evaluationHandlers[jsonParsingHandlerTally + 1]);
                }
            }
        }
    }
}
