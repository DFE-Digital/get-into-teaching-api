using System;
using System.Runtime.Serialization;

namespace GetIntoTeachingApi.Utils
{
    [Serializable]
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

        protected BombFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
