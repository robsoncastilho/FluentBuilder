using Nosbor.FluentBuilder.Internals.Support;
using Nosbor.FluentBuilder.Lib;
using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal class DefaultValueForConcreteClass : IDefaultValueGenerator
    {
        private readonly GenericTypeCreator _genericTypeCreator;

        public DefaultValueForConcreteClass(GenericTypeCreator genericTypeCreator)
        {
            _genericTypeCreator = genericTypeCreator;
        }

        public object GetDefaultValueFor(Type type)
        {
            var builderForChildObject = _genericTypeCreator.CreateInstanceFor(typeof(FluentBuilder<>), type);
            var methodInfo = builderForChildObject.GetType().GetMethod("Build");
            return methodInfo.Invoke(builderForChildObject, new object[] { });
        }
    }
}