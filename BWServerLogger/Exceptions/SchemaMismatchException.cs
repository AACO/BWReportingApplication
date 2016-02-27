using System;

namespace BWServerLogger.Exceptions {
    [Serializable]
    public class SchemaMismatchException : Exception {
        public SchemaMismatchException() : base() {
        }

        public SchemaMismatchException(string message) : base(message) {
        }

        public SchemaMismatchException(string message, Exception inner) : base(message, inner) {
        }
    }
}
