namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common
{
    /// <summary>
    /// Defines a contract for handling evaluation requests following a
    /// 'chain-of-responsibility' pattern. The idea being to provide a
    /// vanilla handler that can orchestrate the chain-of-responsibility
    /// and allow re-use across multiple projects (configured via the DI container).
    /// 
    /// The Chain-of-Responsibility design pattern is a behavioral design pattern that
    /// allows an object to pass a request along a chain of handlers. Each handler in
    /// the chain decides either to process the request or to pass it along the chain to the next handler.
    /// </summary>
    /// <typeparam name="TEvaluationRequest">
    /// Request type that encapsulates the data required for evaluation.
    /// </typeparam>
    /// <typeparam name="TEvaluationResponse">
    /// The response object associated the configured evaluation handlers.
    /// </typeparam>
    public interface IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse>
    {
        /// <summary>
        /// Main call to invoke the evaluation chain.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request type that encapsulates the data required for evaluation.
        /// </param>
        /// <returns>
        /// The response object associated the configured evaluation handlers.
        /// </returns>
        TEvaluationResponse Evaluate(TEvaluationRequest evaluationRequest);

        /// <summary>
        /// Allows handlers to be chained together in a linear evaluation sequence.
        /// </summary>
        /// <param name="nextEvaluator">
        /// The next evaluation handler in the chain.
        /// </param>
        void ChainNextHandler(IChainEvaluationHandler<TEvaluationRequest, TEvaluationResponse> nextEvaluator);
    }
}
