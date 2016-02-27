using System;

namespace BWServerLogger.Exceptions {
    [Serializable]
    public class NoScheduleException : Exception {
        public NoScheduleException() : base() {
        }

        public NoScheduleException(string message) : base(message) {
        }

        public NoScheduleException(string message, Exception inner) : base(message, inner) {
        }
    }
}
