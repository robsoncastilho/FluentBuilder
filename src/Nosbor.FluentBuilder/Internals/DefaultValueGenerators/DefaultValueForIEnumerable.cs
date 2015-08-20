using Nosbor.FluentBuilder.Internals.Support;
using System;
using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal class DefaultValueForIEnumerable : IDefaultValueGenerator
    {
        private readonly GenericTypeCreator _genericTypeCreator;

        public DefaultValueForIEnumerable(GenericTypeCreator genericTypeCreator)
        {
            _genericTypeCreator = genericTypeCreator;
        }

        public object GetDefaultValueFor(Type type)
        {
            return _genericTypeCreator.CreateInstanceFor(typeof(List<>), type.GenericTypeArguments);
        }
    }
}