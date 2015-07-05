using System;

namespace Nosbor.FluentBuilder.Exceptions
{
    public class FluentBuilderException : Exception
    {
        internal FluentBuilderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
