using System;

namespace BWServerLogger.Exceptions
{
    [Serializable]
    public class NoLastInsertedIdException : Exception
    {
        public NoLastInsertedIdException() : base()
        {
        }

        public NoLastInsertedIdException(string message) : base(message)
        {
        }

        public NoLastInsertedIdException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
