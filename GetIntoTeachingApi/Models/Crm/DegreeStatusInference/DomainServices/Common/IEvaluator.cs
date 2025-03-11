namespace GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common
{
    /// <summary>
    /// Defines the behavioural contract for an evaluator that
    /// can be used to evaluate a request and return a response.
    /// </summary>
    /// <typeparam name="TEvaluationRequest">
    /// Request type that encapsulates the data required for evaluation.
    /// </typeparam>
    /// <typeparam name="TEvaluationResponse">
    /// The response object associated the configured evaluation handlers.
    /// </typeparam>
    public interface IEvaluator<in TEvaluationRequest, out TEvaluationResponse>
    {
        /// <summary>
        /// Main call to invoke the evaluation request.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request type that encapsulates the data required for evaluation.
        /// </param>
        /// <returns>
        /// The response object associated the configured evaluation handlers.
        /// </returns>
        TEvaluationResponse Evaluate(TEvaluationRequest evaluationRequest);

        /// <summary>
        /// Allows the evaluator to determine if it can evaluate the request.
        /// </summary>
        /// <param name="evaluationRequest">
        /// Request type that encapsulates the data required for evaluation.
        /// </param>
        /// <returns>
        /// The boolean value indicating if the evaluator can evaluate the request.
        /// </returns>
        bool CanEvaluate(TEvaluationRequest evaluationRequest);
    }
}
