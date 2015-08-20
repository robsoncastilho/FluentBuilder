using System;

namespace Nosbor.FluentBuilder.Internals.Support
{
    internal class GenericTypeCreator
    {
        internal object CreateInstanceFor(Type genericTypeToMake, params Type[] genericTypeArguments)
        {
            var genericType = genericTypeToMake.MakeGenericType(genericTypeArguments);
            return Activator.CreateInstance(genericType);
        }
    }
}