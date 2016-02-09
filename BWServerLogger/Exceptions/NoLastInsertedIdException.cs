using System;

namespace BWServerLogger.Exceptions
{
    [Serializable]
    public class NoLastInsertedIdException : Exception
    {
        public NoLastInsertedIdException()
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
