using System;

namespace Nosbor.FluentBuilder.Exceptions
{
    public class FluentBuilderException : Exception
    {
        public FluentBuilderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
