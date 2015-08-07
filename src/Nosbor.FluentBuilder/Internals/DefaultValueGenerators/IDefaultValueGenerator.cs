using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    public interface IDefaultValueGenerator
    {
        object GetDefaultValueFor(Type type);
    }
}