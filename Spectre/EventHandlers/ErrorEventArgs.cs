using System;
using System.Diagnostics;

namespace Spectre.EventHandlers
{
    /// <summary>
    /// Provides data for the SysError events.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Occured error's Exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Occured error's Stack Trace.
        /// </summary>
        public StackTrace StackTrace { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref=" ErrorEventArgs"/>.
        /// </summary>
        /// <param name="exception">Occured error's Exception.</param>
        /// <param name="stackTrace">Occured error's Stack Trace.</param>
        public ErrorEventArgs(Exception exception, StackTrace stackTrace)
        {
            Exception = exception;
            StackTrace = stackTrace;
        }
    }
}
