using System;

namespace GetIntoTeachingApi.Utils
{
    public class BombFoundException : Exception
    {
        public BombFoundException()
        {
        }

        public BombFoundException(string message)
            : base(message)
        {
        }

        public BombFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
