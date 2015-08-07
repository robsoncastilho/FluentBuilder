using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal class DefaultValueGeneratorFactory : IDefaultValueGeneratorFactory
    {
        public IDefaultValueGenerator CreateFor(Type type)
        {
            if (type == typeof(string))
                return new DefaultValueForString();

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return new DefaultValueForIEnumerable(new GenericTypeCreator());

            if (type.IsClass && !type.IsAbstract)
                return new DefaultValueForConcreteClass();

            return null;//TODO: null?
        }
    }
}
