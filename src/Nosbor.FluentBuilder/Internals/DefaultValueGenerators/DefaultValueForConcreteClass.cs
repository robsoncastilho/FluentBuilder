using Nosbor.FluentBuilder.Lib;
using System;

namespace Nosbor.FluentBuilder.Internals.DefaultValueGenerators
{
    internal class DefaultValueForConcreteClass : IDefaultValueGenerator
    {
        public object GetDefaultValueFor(Type type)
        {
            var typeOfBuilder = typeof(FluentBuilder<>).MakeGenericType(type);
            var builderForChildObject = Activator.CreateInstance(typeOfBuilder);
            var methodInfo = typeOfBuilder.GetMethod("Build");
            return methodInfo.Invoke(builderForChildObject, new object[] { });
        }
    }
}