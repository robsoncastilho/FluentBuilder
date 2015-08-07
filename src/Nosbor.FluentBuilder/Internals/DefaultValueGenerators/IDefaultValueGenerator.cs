using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal interface IDefaultValueGenerator
    {
        object GetDefaultValueFor(Type type);
    }
}