namespace GetIntoTeaching.Core.Infrastructure.BackgroundProcessing
{
    internal interface IBackgroundProcessHandler
    {
        TResult InvokeProcessor<TResult>();
    }
}
