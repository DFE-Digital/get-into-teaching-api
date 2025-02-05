using Serilog.Events;

namespace GetIntoTeachingApiTests.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers.Extensions
{
    public static class LogEventExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturnValue"></typeparam>
        /// <param name="logEvent"></param>
        /// <param name="propertyKey"></param>
        /// <returns></returns>
        public static TReturnValue GetScalarValue<TReturnValue>(
            this LogEvent logEvent, string propertyKey) where TReturnValue : class =>
                (logEvent.Properties.TryGetValue(
                    propertyKey, out LogEventPropertyValue logEventPropertyValue) &&
                    logEventPropertyValue is ScalarValue scalarValue) ?
                        scalarValue.Value as TReturnValue: default;
    }
}
