using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    public interface IDefaultValueGeneratorFactory
    {
        IDefaultValueGenerator CreateFor(Type type);
    }
}
