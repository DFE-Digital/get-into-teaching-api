using Serilog.Core;
using Serilog.Events;
using System;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    public static class LogEventExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="propertyFactory"></param>
        /// <param name="propertyKey"></param>
        /// <param name="propertyValue"></param>
        public static void LogProperty(
            this LogEvent logEvent,
            ILogEventPropertyFactory propertyFactory,
            string propertyKey,
            object propertyValue)
        {
            ArgumentNullException.ThrowIfNull(logEvent);
            ArgumentNullException.ThrowIfNull(propertyFactory);
            ArgumentNullException.ThrowIfNullOrEmpty(propertyKey);

            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty(propertyKey, propertyValue));
        }
    }
}
