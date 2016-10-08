using Nosbor.FluentBuilder.Internals.DefaultValueGenerators;
using System;
using System.Reflection;
#if NET451
    using System.Runtime.Serialization;
#endif

namespace Nosbor.FluentBuilder.Internals.Support
{
    internal static class TypeExtensions
    {
        internal static IDefaultValueGenerator GetDefaultValueGenerator(this Type type)
        {
            if (type.IsString())
                return new DefaultValueForString();

            if (!type.IsArray && typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return new DefaultValueForIEnumerable(new GenericTypeCreator());

            return null; //TODO: null?
        }

        internal static bool IsAllowedToInitialize(this Type type, Type destinationObjectType)
        {
            return type != destinationObjectType && !type.IsArray && (type.IsString() || typeof(System.Collections.IEnumerable).IsAssignableFrom(type));
        }

        internal static bool IsInstanceOfTypeBase(this Type type, object parentType)
        {
            return type.IsInstanceOfType(parentType);
        }

        internal static bool IsString(this Type type)
        {
            return type == typeof(string);
        }

        internal static Type BaseType(this Type type)
        {

#if NET451
            return type.BaseType;
#else
            return type.GetTypeInfo().BaseType;
#endif
        }

        internal static T CreateInstance<T>(this Type type)
        {
#if NET451
            return (T)FormatterServices.GetUninitializedObject(typeof(T));
#else
            return (T)GetUninitializedObjectWithFormatterServices(typeof(T));
#endif
        }

        private static object GetUninitializedObjectWithFormatterServices(Type type)
        {
            return GetUninitializedObjectDelegate.Invoke(type);
        }

        private static readonly Func<Type, object> GetUninitializedObjectDelegate =
            (Func<Type, object>)
                typeof (string)
                    .GetTypeInfo()
                    .Assembly
                    .GetType("System.Runtime.Serialization.FormatterServices")
                    .GetMethod("GetUninitializedObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .CreateDelegate(typeof (Func<Type, object>));
    }
}
