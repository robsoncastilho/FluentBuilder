using System;

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
            return _genericTypeCreator.CreateInstanceFor(type.GenericTypeArguments);
        }
    }
}
