using System;

namespace BWServerLogger.Exceptions {
    /// <summary>
    /// Exception thrown when there is no retries left for an operation
    /// </summary>
    [Serializable]
    public class NoRetriesLeftException : Exception {
        /// <summary>
        /// Default exception thrown constructor
        /// </summary>
        public NoRetriesLeftException() : base() {
        }

        /// <summary>
        /// Exception thrown constructor with additional message
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        public NoRetriesLeftException(string message) : base(message) {
        }

        /// <summary>
        /// Exception thrown constructor with additional message and inner exception
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        /// <param name="inner">Inner exception to add detail to the current exception</param>
        public NoRetriesLeftException(string message, Exception inner) : base(message, inner) {
        }
    }
}
