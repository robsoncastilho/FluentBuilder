using System;

namespace Nosbor.FluentBuilder.Internals.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsString(this Type type)
        {
            return type == typeof(string);
        }

        internal static bool IsConcreteClass(this Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }

        internal static bool InheritsFrom<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}
