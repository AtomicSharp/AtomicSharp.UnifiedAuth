using System;
using System.Runtime.Serialization;

namespace Atomic.ExceptionHandling
{
    /// <summary>
    /// Base exception type for those are thrown by Atomic system for Atomic specific exceptions.
    /// </summary>
    public class AtomicException : Exception
    {
        public AtomicException()
        {
        }

        public AtomicException(string message)
            : base(message)
        {
        }

        public AtomicException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AtomicException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}