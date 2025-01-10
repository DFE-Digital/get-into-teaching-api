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
        /// <param name="request"></param>
        void Process(IBackgroundProcessorRequest request);
    }
}
