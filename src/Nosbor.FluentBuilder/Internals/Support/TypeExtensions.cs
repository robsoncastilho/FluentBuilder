using Nosbor.FluentBuilder.Internals.DefaultValueGenerators;
using System;
using System.Reflection;
#if NET45
    using System.Runtime.Serialization;
#endif

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

        public static Type BaseType(this Type type)
        {

#if NET45
            return type.BaseType;
#else
            return type.GetTypeInfo().BaseType;
#endif
        }

        public static bool IsInstanceOfTypeBase(this Type type, object parentType)
        {
            return type.IsInstanceOfType(parentType);
        }

        private static readonly Func<Type, object> GetUninitializedObjectDelegate =
          (Func<Type, object>)
              typeof(string)
                  .GetTypeInfo()
                  .Assembly
                  .GetType("System.Runtime.Serialization.FormatterServices")
                  .GetMethod("GetUninitializedObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                  .CreateDelegate(typeof(Func<Type, object>));

        private static object GetUninitializedObjectWithFormatterServices(Type type)
        {
            return GetUninitializedObjectDelegate.Invoke(type);
        }

        public static T CreateInstance<T>(this Type type)
        {
#if NET45
            return (T)FormatterServices.GetUninitializedObject(typeof(T));
#else
            return (T)GetUninitializedObjectWithFormatterServices(typeof(T));
#endif
        }
    }
}
