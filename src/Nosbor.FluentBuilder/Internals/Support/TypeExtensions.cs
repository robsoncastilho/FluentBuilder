using Nosbor.FluentBuilder.Internals.DefaultValueGenerators;
using System;

namespace Nosbor.FluentBuilder.Internals.Support
{
    internal static class TypeExtensions
    {
        internal static IDefaultValueGenerator GetDefaultValueGenerator(this Type type)
        {
            if (IsString(type))
                return new DefaultValueForString();

            if (InheritsFrom<System.Collections.IEnumerable>(type))
                return new DefaultValueForIEnumerable(new GenericTypeCreator());

            return null; //TODO: null?
        }

        internal static bool IsAllowedToInitialize(this Type type, Type destinationObjectType)
        {
            return type != destinationObjectType && (IsString(type) || InheritsFrom<System.Collections.IEnumerable>(type));
        }

        private static bool IsString(Type type)
        {
            return type == typeof(string);
        }

        private static bool InheritsFrom<T>(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}
