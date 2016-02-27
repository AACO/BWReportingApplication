using System;

namespace BWServerLogger.Exceptions {
    [Serializable]
    public class NoSchemaException : Exception {
        public NoSchemaException() : base() {
        }

        public NoSchemaException(string message) : base(message) {
        }

        public NoSchemaException(string message, Exception inner) : base(message, inner) {
        }
    }
}
