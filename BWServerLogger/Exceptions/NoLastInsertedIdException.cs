using System;

namespace BWServerLogger.Exceptions {
    /// <summary>
    /// Exception thrown when there is no last inserted ID in the database
    /// </summary>
    [Serializable]
    public class NoLastInsertedIdException : Exception {
        /// <summary>
        /// Default exception thrown constructor
        /// </summary>
        public NoLastInsertedIdException() : base() {
        }

        /// <summary>
        /// Exception thrown constructor with additional message
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        public NoLastInsertedIdException(string message) : base(message) {
        }

        /// <summary>
        /// Exception thrown constructor with additional message and inner exception
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        /// <param name="inner">Inner exception to add detail to the current exception</param>
        public NoLastInsertedIdException(string message, Exception inner) : base(message, inner) {
        }
    }
}
