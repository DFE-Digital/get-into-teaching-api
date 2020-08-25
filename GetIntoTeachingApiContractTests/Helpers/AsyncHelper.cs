using System;
using System.Threading;
using System.Threading.Tasks;

namespace GetIntoTeachingApiContractTests.Helpers
{
    public static class AsyncHelper  
    {
        private static readonly TaskFactory TASK_FACTORY = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            => TASK_FACTORY
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static void RunSync(Func<Task> func)
            => TASK_FACTORY
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }
}