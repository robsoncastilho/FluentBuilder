using System;
using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Internals
{
    internal class GenericTypeCreator
    {
        internal object CreateInstanceFor(Type[] genericTypeArguments)
        {
            var genericListType = typeof(List<>).MakeGenericType(genericTypeArguments);
            return Activator.CreateInstance(genericListType);
        }
    }
}