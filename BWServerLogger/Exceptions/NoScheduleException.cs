using System;

namespace BWServerLogger.Exceptions {
    /// <summary>
    /// Exception thrown when there is no schedule for a job
    /// </summary>
    [Serializable]
    public class NoScheduleException : Exception {
        /// <summary>
        /// Default exception thrown constructor
        /// </summary>
        public NoScheduleException() : base() {
        }

        /// <summary>
        /// Exception thrown constructor with additional message
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        public NoScheduleException(string message) : base(message) {
        }

        /// <summary>
        /// Exception thrown constructor with additional message and inner exception
        /// </summary>
        /// <param name="message">Message to add to exception</param>
        /// <param name="inner">Inner exception to add detail to the current exception</param>
        public NoScheduleException(string message, Exception inner) : base(message, inner) {
        }
    }
}
