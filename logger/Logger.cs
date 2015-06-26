// ---------------------------------------------------------------
// 
// Copyright © Microsoft. All rights reserved.
// 
// ---------------------------------------------------------------

namespace Microsoft.Msn.Utilities.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Foundation.Diagnostics;

    /// <summary>
    /// Implementation of the wrapper for Logging service
    /// </summary>
    public class Logger : IHostLoggerService
    {
        private static Lazy<Logger> logger = new Lazy<Logger>(() => new Logger());
        private static bool disposed = false;
        private SemaphoreSlim semaphore = null;
        private LoggingLevel defaultLoggingLevel = LoggingLevel.Verbose;
        private IList<IHostLogListener> logListeners = new List<IHostLogListener>();
        // Holds whether to defer the formatting of the message. Will not defer formatting, if atleast 1 listener has 'No' to defer formatting
        private bool deferFormatMessage = true;

        /// <summary>
        /// Singleton 
        /// </summary>
        private Logger()
        {
        }

        /// <summary>
        /// Gets or sets if logging is enabled
        /// </summary>
        public bool IsEnabled
        {
            get; internal set;
        }

        /// <summary>
        /// Returns the singleton instance of the logger
        /// </summary>
        public static Logger Instance
        {
            get
            {
                return logger.Value;
            }
        }

        /// <summary>
        /// Returns the registered log listeners.
        /// </summary>
        internal IList<IHostLogListener> Listeners
        {
            get
            {
                return this.logListeners;
            }
        }

        /// <summary>
        /// Gets or sets the minimum log level
        /// </summary>
        internal LoggingLevel MinLogLevel
        {
            get
            {
                return this.defaultLoggingLevel;
            }

            set
            {
                this.SetMinLogLevel(value);
            }
        }

        /// <summary>
        /// Changes the logging level for all the categories
        /// </summary>
        /// <param name="level">The logging level</param>
        public void SetMinLogLevel(LoggingLevel level)
        {
            this.defaultLoggingLevel = level;

            foreach (IHostLogListener listener in this.logListeners)
            {
                listener.SetMinLogLevel(this.defaultLoggingLevel);
            }
        }

        /// <summary>
        /// Adds a listener to the logger
        /// </summary>
        /// <param name="listener">The listener for the logger</param>
        public void AddListener(IHostLogListener listener)
        {
            if (semaphore == null)
            {
                semaphore = new SemaphoreSlim(1);
            }
            semaphore.Wait();
            if (IsEnabled)
            {
                //Explicitly re setting the dispose, to handle the case when a new listener is added
                disposed = false;
                listener.SetMinLogLevel(defaultLoggingLevel);
                logListeners.Add(listener);
                //Resetting the deferFormatMessage (based on the remaining listeners)
                deferFormatMessage = listener.DeferFormatMessage && this.deferFormatMessage;
            }
            semaphore.Release();
        }

        /// <summary>
        /// Removes a listener to the logger
        /// </summary>
        /// <param name="listener">The listener to be removed</param>
        public void RemoveListener(IHostLogListener listener)
        {
            if (semaphore == null)
            {
                semaphore = new SemaphoreSlim(1);
            }
            semaphore.Wait();
            if (IsEnabled)
            {
                if (logListeners.Contains(listener))
                {
                    logListeners.Remove(listener);
                    deferFormatMessage = logListeners.Select(x => x.DeferFormatMessage == false).Count() > 0 ? false: true;
                }
            }
            semaphore.Release();
        }

        /// <summary>
        /// Writes the message at the specified logging level
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="message">The message to be logged</param>
        public void LogMessage(LoggingLevel level, string message)
        {
            if (IsEnabled)
            {
                foreach (IHostLogListener listener in logListeners)
                {
                    listener.WriteLog(LoggingConstants.DefaultCategory, level, message);
                }
            }
        }
        
        /// <summary>
        /// Writes the message to the specified category
        /// </summary>
        /// <param name="categoryName">The category of the log</param>
        /// <param name="level">The logging level</param>
        /// <param name="message">The message to be logged</param>
        public void LogMessage(string categoryName, LoggingLevel level, string message)
        {
            if (IsEnabled)
            {
                foreach (IHostLogListener listener in logListeners)
                {
                    listener.WriteLog(categoryName, level, message);
                }
            }
        }

        /// <summary>
        /// Writes the exception at the specified logging level
        /// </summary>
        /// <param name="level">The logging level</param>
        /// <param name="exception">The exception to be logged</param>
        public void LogMessage(LoggingLevel level, Exception exception)
        {
            if (IsEnabled)
            {
                foreach (IHostLogListener listener in logListeners)
                {
                    listener.WriteLog(LoggingConstants.DefaultCategory, level, exception.ToString());
                }
            }
        }

        /// <summary>
        /// Writes the exception to the specified category
        /// </summary>
        /// <param name="categoryName">The category of the log</param>
        /// <param name="level">The logging level</param>
        /// <param name="exception">The exception to be logged</param>
        public void LogMessage(string categoryName, LoggingLevel level, Exception exception)
        {
            if (IsEnabled)
            {
                foreach (IHostLogListener listener in logListeners)
                {
                    listener.WriteLog(categoryName, level, exception.ToString());
                }
            }
        }

        /// <summary>
        /// Writes the message returned by delegate at the specified logging level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void LogMessage(LoggingLevel level, string format, params object[] args)
        {
            this.LogMessage(LoggingConstants.DefaultCategory, level, format, args);
        }        

        /// <summary>
        /// Writes the formatted message returned by delegate to the specified category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="parameters"></param>
        public void LogMessage(string categoryName, LoggingLevel level, string format, params object[] args)
        {
            if (!IsEnabled)
            {
                return;
            }

            string message;

            if (categoryName.IsNullOrWhiteSpace())
            {
                categoryName = LoggingConstants.DefaultCategory;
            }

            if (!deferFormatMessage)
            {
                message = this.FormatString(format, args);

                foreach (var listener in logListeners)
                {
                    listener.WriteLog(categoryName, level, message);
                }
            }
            else
            {
                foreach (IHostLogListener listener in logListeners)
                {
                    listener.WriteLog(categoryName, level, FormatString, format, args);
                }
            }            
        }
        
        /// <summary>
        /// Saves the logs
        /// </summary>
        public async Task SaveLogsAsync()
        {
            if (semaphore == null)
            {
                semaphore = new SemaphoreSlim(1);
            }

            await this.semaphore.WaitAsync().ConfigureAwait(false);

            var listenersClone = this.logListeners.ToList();

            this.semaphore.Release();

            var tasks = listenersClone.Select(l => l.SaveLogsAsync());
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Disposes the listeners
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose both managed and unmanaged resources
        /// </summary>
        /// <param name="disposing">Specifies whether the call is coming from Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                semaphore.Dispose();
                semaphore = null;
                foreach (IHostLogListener listener in logListeners)
                {
                    try
                    {
                        listener.Dispose();
                    }
                    catch (Exception)
                    {
                        if (Debugger.IsAttached)
                        {
                            // An unhandled exception has occurred; break into the debugger
                            Debugger.Break();
                        }
                    }
                }

                logListeners.Clear();
                disposed = true;
                IsEnabled = false;
            }
        }

        /// <summary>
        /// Outputs a message when the condition is false
        /// </summary>
        /// <param name="condition">the condition</param>
        /// <param name="level">Log level</param>
        /// <param name="message">Log message</param>
        public void LogIfFalse(bool condition, LoggingLevel level, string message = null)
        {          
            if (!condition)
            {
                foreach (IHostLogListener listener in logListeners)
                {
                    listener.WriteLog(level, message);
                }
            }
        }
        
        /// <summary>
        /// Returns the formatted string
        /// </summary>
        /// <param name="format">The format of the string</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        private string FormatString(string format, object[] parameters)
        {
            return string.Format(format, parameters);
        }
    }
}