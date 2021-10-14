using System;

namespace IntegreNet.Exceptions
{
    public class IntegreException : Exception
    {
        public IntegreException(string message) : base(message) { }

        public IntegreException(string message, Exception innerException) : base(message, innerException) { }
    }
}