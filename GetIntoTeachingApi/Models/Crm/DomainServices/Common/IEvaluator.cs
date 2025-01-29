namespace GetIntoTeachingApi.Models.Crm.DomainServices.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEvaluationRequest"></typeparam>
    /// <typeparam name="TEvaluationResponse"></typeparam>
    public interface IEvaluator<in TEvaluationRequest, out TEvaluationResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        TEvaluationResponse Evaluate(TEvaluationRequest evaluationRequest);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationRequest"></param>
        /// <returns></returns>
        bool CanEvaluate(TEvaluationRequest evaluationRequest);
    }
}
