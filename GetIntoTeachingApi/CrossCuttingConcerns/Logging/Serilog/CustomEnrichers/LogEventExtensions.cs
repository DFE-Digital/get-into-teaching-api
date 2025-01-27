using Serilog.Core;
using Serilog.Events;
using System;

namespace GetIntoTeachingApi.CrossCuttingConcerns.Logging.Serilog.CustomEnrichers
{
    /// <summary>
    /// Extension method used to aggregate the behaviour required to
    /// create a new log property, and add to the current <see cref="LogEvent"/> property collection.
    /// </summary>
    public static class LogEventExtensions
    {
        /// <summary>
        /// Allows additional properties to be added to a given
        /// <see cref="LogEvent"/> property collection.
        /// </summary>
        /// <param name="logEvent">
        /// The <see cref="LogEvent"/> instance whose properties will be extended.
        /// </param>
        /// <param name="propertyFactory">
        /// The <see cref="ILogEventPropertyFactory"/> instance used to create log event
        /// properties from regular .NET objects,as required.
        /// </param>
        /// <param name="propertyKey">
        /// The string values used to assign as the property name.
        /// </param>
        /// <param name="propertyValue">
        /// The object value used to assign as the property value.
        /// </param>
        /// <returns></returns>
        public static LogEvent LogProperty(
            this LogEvent logEvent,
            ILogEventPropertyFactory propertyFactory,
            string propertyKey,
            object propertyValue)
        {
            ArgumentNullException.ThrowIfNull(logEvent);
            ArgumentNullException.ThrowIfNull(propertyFactory);
            ArgumentNullException.ThrowIfNull(propertyValue);

            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty(propertyKey, propertyValue));

            return logEvent;
        }
    }
}
