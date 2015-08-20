using Nosbor.FluentBuilder.Internals.Support;
using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal class DefaultValueGeneratorFactory : IDefaultValueGeneratorFactory
    {
        public IDefaultValueGenerator CreateFor(Type type)
        {
            if (type.IsString())
                return new DefaultValueForString();

            if (type.InheritsFrom<System.Collections.IEnumerable>())
                return new DefaultValueForIEnumerable(new GenericTypeCreator());

            if (type.IsConcreteClass())
                return new DefaultValueForConcreteClass(new GenericTypeCreator());

            return null; //TODO: null?
        }
    }
}
