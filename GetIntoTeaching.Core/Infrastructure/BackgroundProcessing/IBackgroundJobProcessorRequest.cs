using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator;

namespace GetIntoTeaching.Infrastructure.BackgroundProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackgroundJobProcessorRequest :
        IRequest<BackgroundProcessorResult>{
    }
}
