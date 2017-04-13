using System;

namespace Spectre
{
    public class ReadException : Exception
    {
        /// <summary>
        /// Read error code.
        /// </summary>
        public ReadReplyCode ReadReplyCode { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ReadException"/> exception. 
        /// </summary>
        /// <param name="code">Read error code.</param>
        /// <param name="message">The message that describes the error.</param>
        public ReadException(ReadReplyCode code, string message) : base(message)
        {
            ReadReplyCode = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ReadException"/> exception. 
        /// </summary>
        /// <param name="code">Read error code.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ReadException(ReadReplyCode code, string message, Exception innerException) :base(message, innerException)
        {
            ReadReplyCode = code;
        }
    }
}
