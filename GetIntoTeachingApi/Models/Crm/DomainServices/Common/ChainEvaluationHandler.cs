using System;

namespace GetIntoTeachingApi.Models.Crm.DomainServices.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEvaluationRequest"></typeparam>
    /// <typeparam name="TEvaluationResponse"></typeparam>
    public sealed class ChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> : IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse>
    {
        private IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> _nextEvaluationHandler;
        private readonly IEvaluator<TEvaluationRequest, TEvaluationResponse> _evaluator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluator"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ChainEvaluationHandler(IEvaluator<TEvaluationRequest, TEvaluationResponse> evaluator)
        {
            _evaluator = evaluator ??
                throw new ArgumentNullException(nameof(evaluator));

            _nextEvaluationHandler = RootEvaluationHandler.CreateRoot();    // Set root handler as default.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public TEvaluationResponse Evaluate(TEvaluationRequest evaluationRequest)
        {
            if (evaluationRequest == null)
            {
                throw new ArgumentNullException(nameof(evaluationRequest));
            }

            return _evaluator.CanEvaluate(evaluationRequest) ?
                _evaluator.Evaluate(evaluationRequest) :
                _nextEvaluationHandler.Evaluate(evaluationRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextEvaluator"></param>
        public void ChainNextHandler(IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> nextEvaluator) =>
            _nextEvaluationHandler = nextEvaluator;

        /// <summary>
        /// 
        /// </summary>
        internal sealed class RootEvaluationHandler : IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="evaluationRequest"></param>
            /// <returns></returns>
            public TEvaluationResponse Evaluate(TEvaluationRequest evaluationRequest) => default!;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="nextEvaluator"></param>
            /// <exception cref="InvalidOperationException"></exception>
            public void ChainNextHandler(IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> nextEvaluator) =>
                throw new InvalidOperationException("Root evaluation handler must be invoked last.");

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static RootEvaluationHandler CreateRoot() => new();
        }
    }
}
