using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal interface IDefaultValueGeneratorFactory
    {
        IDefaultValueGenerator CreateFor(Type type);
    }
}
