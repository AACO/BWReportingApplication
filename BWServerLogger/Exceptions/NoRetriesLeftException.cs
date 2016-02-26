using System;

namespace BWServerLogger.Exceptions
{
    [Serializable]
    public class NoRetriesLeftException : Exception
    {
        public NoRetriesLeftException() : base()
        {
        }

        public NoRetriesLeftException(string message) : base(message)
        {
        }

        public NoRetriesLeftException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
