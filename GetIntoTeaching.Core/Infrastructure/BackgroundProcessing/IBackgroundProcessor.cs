namespace GetIntoTeaching.Infrastructure.Persistence.CandidateEventProcessing.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackgroundProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        TResult Process<TResult>(IBackgroundProcessorRequest request);
    }
}
