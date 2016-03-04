using System;

namespace BWServerLogger.Exceptions {
    /// <summary>
    /// Exception thrown when there is an issue retrieving database schema
    /// </summary>
    [Serializable]
    public class NoSchemaException : Exception {
        /// <summary>
        /// Default exception thrown constructor
        /// </summary>
        public NoSchemaException() : base() {
        }

        /// <summary>
        /// Exception thrown constructor with additional message
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        public NoSchemaException(string message) : base(message) {
        }

        /// <summary>
        /// Exception thrown constructor with additional message and inner exception
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        /// <param name="inner">Inner exception to add detail to the current exception</param>
        public NoSchemaException(string message, Exception inner) : base(message, inner) {
        }
    }
}
