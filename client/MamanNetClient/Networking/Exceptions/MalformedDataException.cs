using System;
using System.Runtime.Serialization;

namespace Networking.Exceptions
{
    [Serializable]
    internal class MalformedDataException : Exception
    {
        public MalformedDataException()
        {
        }

        public MalformedDataException(string message) : base(message)
        {
        }

        public MalformedDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MalformedDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}