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
    /// Interface for the log listeners
    /// </summary>
    public interface IHostLogListener : IDisposable
    {
        /// <summary>
        /// Gets or sets deferring status to format the message
        /// </summary>
        bool DeferFormatMessage 
        {
            get; 
            set; 
        }

        /// <summary>
        /// Writes the log message to the default category / channel
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="message">The message to be logged</param>
        void WriteLog(LoggingLevel level, string message);
        
        /// <summary>
        /// Writes the log message to the default category / channel
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="message">The message to be logged</param>
        void WriteLog(string categoryName, LoggingLevel level, Func<string, object[], string> formatMessageDelegate, string format, params object[] args);

        /// <summary>
        /// Writes the log message to the specified category
        /// </summary>
        /// <param name="categoryName">The category of the log</param>
        /// <param name="level">The logging level</param>
        /// <param name="message">The message to be logged</param>
        void WriteLog(string categoryName, LoggingLevel level, string message);       

        /// <summary>
        /// Writes the exception to the default category
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="exception">The exception to be logged</param>
        void WriteLog(LoggingLevel level, Exception exception);

        /// <summary>
        /// Writes the exception to the specified category
        /// </summary>
        /// <param name="categoryName">The category of the log</param>
        /// <param name="level">The logging level</param>
        /// <param name="exception">The exception to be logged</param>
        void WriteLog(string categoryName, LoggingLevel level, Exception exception);

        /// <summary>
        /// Changes the logging level for all the categories
        /// </summary>
        /// <param name="level">The logging level</param>
        void SetMinLogLevel(LoggingLevel level);

        /// <summary>
        /// Saves the in-memory logs to the file
        /// </summary>
        /// <returns></returns>
        Task SaveLogsAsync();
    }
}