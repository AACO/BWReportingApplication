using System;

namespace BWServerLogger.Exceptions {
    /// <summary>
    /// Exception thrown when there is issues getting server information
    /// </summary>
    [Serializable]
    public class NoServerInfoException : Exception {
        /// <summary>
        /// Default exception thrown constructor
        /// </summary>
        public NoServerInfoException() : base() {
        }

        /// <summary>
        /// Exception thrown constructor with additional message
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        public NoServerInfoException(string message) : base(message) {
        }

        /// <summary>
        /// Exception thrown constructor with additional message and inner exception
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        /// <param name="inner">Inner exception to add detail to the current exception</param>
        public NoServerInfoException(string message, Exception inner) : base(message, inner) {
        }
    }
}
