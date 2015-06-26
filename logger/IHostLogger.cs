// ---------------------------------------------------------------
// 
// Copyright © Microsoft. All rights reserved.
// 
// ---------------------------------------------------------------

namespace Microsoft.Msn.Utilities
{
    using System;
    using System.Threading.Tasks;
    using Windows.Foundation.Diagnostics;

    /// <summary>
    /// Interface for the logging framework
    /// </summary>
    public interface IHostLoggerService : IDisposable 
    {
        /// <summary>
        /// Writes the message to the specified category
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="message">The message to be logged</param>
        void LogMessage(LoggingLevel level, string message);

        /// <summary>
        /// Writes the message to the specified category
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="format">The mesage format</param>
        /// <param name="args">Args of the msg format</param>
        void LogMessage(LoggingLevel level, string format, params object[] args);

        /// <summary>
        /// Writes the exception to the specified category
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="exception">The exception to be logged</param>
        void LogMessage(LoggingLevel level, Exception exception);

        /// <summary>
        /// Writes the message to the specified category
        /// </summary>
        /// <param name="categoryName">The category of the log</param>
        /// <param name="level">The logging level</param>
        /// <param name="message">The message to be logged</param>
        void LogMessage(string categoryName, LoggingLevel level, string message);

        /// <summary>
        /// Writes the message to the specified category
        /// </summary>
        /// <param name="categoryName">The category of the log</param>
        /// <param name="level">The logging level</param>
        /// <param name="format">The format of the message</param>
        /// <param name="args">The parameters for formatting the message</param>
        void LogMessage(string categoryName, LoggingLevel level, string format, params object[] args);

        /// <summary>
        /// Writes the exception to the specified category
        /// </summary>
        /// <param name="categoryName">The category of the log</param>
        /// <param name="level">The logging level</param>
        /// <param name="exception">The exception to be logged</param>
        void LogMessage(string categoryName, LoggingLevel level, Exception exception);

        /// <summary>
        /// Outputs a message if the condition fails
        /// </summary>
        /// <param name="condition">the condition</param>
        /// <param name="level">Log level</param>
        /// <param name="message">Log message</param>
        void LogIfFalse(bool condition, LoggingLevel level, string message);

        /// <summary>
        /// Adds a listener to the logger
        /// </summary>
        /// <param name="listener"></param>
        void AddListener(IHostLogListener listener);

        /// <summary>
        /// Removes a listener from the logger
        /// </summary>
        /// <param name="listener"></param>
        void RemoveListener(IHostLogListener listener);

        /// <summary>
        /// Saves the logs
        /// </summary>
        Task SaveLogsAsync();
    }
}
