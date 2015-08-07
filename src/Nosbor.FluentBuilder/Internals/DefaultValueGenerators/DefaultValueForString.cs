using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal class DefaultValueForString : IDefaultValueGenerator
    {
        public object GetDefaultValueFor(Type type)
        {
            return string.Empty;
        }
    }
}
