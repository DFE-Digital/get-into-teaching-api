using System;

namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common
{
    /// <summary>
    /// <para>
    /// Implementation for handling evaluation requests following a
    /// 'chain-of-responsibility' pattern. The idea being to provide a
    /// vanilla handler that can orchestrate the chain-of-responsibility
    /// and allow re-use across multiple projects (configured via the DI container).
    /// </para>
    /// <para>
    /// The Chain-of-Responsibility design pattern is a behavioral design pattern that
    /// allows an object to pass a request along a chain of handlers. Each handler in
    /// the chain decides either to process the request or to pass it along the chain to the next handler.
    /// </para>
    /// </summary>
    /// <typeparam name="TEvaluationRequest">
    /// Request type that encapsulates the data required for evaluation.
    /// </typeparam>
    /// <typeparam name="TEvaluationResponse">
    /// The response object associated the configured evaluation handlers.
    /// </typeparam>
    public sealed class ChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> :
        IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse>
    {
        private IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> _nextEvaluationHandler;
        private readonly IEvaluator<TEvaluationRequest, TEvaluationResponse> _evaluator;

        /// <summary>
        /// Constructor is injected with the <see cref="IEvaluator{TEvaluationRequest, TEvaluationResponse}"/> instance."/>
        /// </summary>
        /// <param name="evaluator">
        /// The <see cref="IEvaluator{TEvaluationRequest, TEvaluationResponse}"/> under which evaluation occurs."/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <see cref="IEvaluator{TEvaluationRequest, TEvaluationResponse}"/> is null.
        /// </exception>
        public ChainEvaluationHandler(IEvaluator<TEvaluationRequest, TEvaluationResponse> evaluator)
        {
            _evaluator = evaluator ??
                throw new ArgumentNullException(nameof(evaluator));

            _nextEvaluationHandler = RootEvaluationHandler.CreateRoot();    // Set root handler as default.
        }

        /// <summary>
        /// Main call to invoke the evaluation chain.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request type that encapsulates the data required for evaluation.
        /// </param>
        /// <returns>
        /// The response object associated the configured evaluation handlers.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <see cref="TEvaluationRequest"/> is null.
        /// </exception>
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
        /// Allows handlers to be chained together in a linear evaluation sequence.
        /// </summary>
        /// <param name="nextEvaluator">
        /// The next evaluation handler in the chain.
        /// </param>
        public void ChainNextHandler(IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> nextEvaluator) =>
            _nextEvaluationHandler = nextEvaluator;

        /// <summary>
        /// Provides a default root evaluation handler which is invoked
        /// last in the chain, if other evaluation conditions are not met.
        /// </summary>
        internal sealed class RootEvaluationHandler : IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse>
        {
            /// <summary>
            /// Main call to invoke the evaluation chain set to the default response type.
            /// </summary>
            /// <param name="evaluationRequest">
            /// Request type that encapsulates the data required for evaluation.
            /// </param>
            /// <returns>
            /// The response object associated the configured evaluation handlers.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Exception thrown if the <see cref="TEvaluationRequest"/> is null.
            /// </exception>
            public TEvaluationResponse Evaluate(TEvaluationRequest evaluationRequest) => default!;

            /// <summary>
            /// Root handler does not support any further chaining.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            /// Exception thrown if an attempt is made to chain another handler to the root.
            /// </exception>
            public void ChainNextHandler(IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> nextEvaluator) =>
                throw new InvalidOperationException("Root evaluation handler must be invoked last.");

            /// <summary>
            /// Factory method to allow easy creation of the root evaluation handler.
            /// </summary>
            /// <returns>
            /// A configured instance of the root evaluation handler.
            /// </returns>
            public static RootEvaluationHandler CreateRoot() => new();
        }
    }
}
