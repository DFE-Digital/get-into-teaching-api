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
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        TResult Process<TEntity, TResult>(TEntity entity);
    }
}
