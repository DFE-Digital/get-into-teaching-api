namespace GetIntoTeaching.Core.Infrastructure.BackgroundProcessing
{
    public interface IBackgroundProcessHandler
    {
        TResult InvokeProcessor<TResult>();
    }
}
