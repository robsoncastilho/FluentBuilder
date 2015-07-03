using Nosbor.FluentBuilder.Lib;
using System;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals
{
    internal class ConstrutorMembersInitializer<T> where T : class
    {
        private readonly MemberSetter<T> _memberSetter = new MemberSetter<T>();
        private readonly GenericTypeCreator _genericTypeCreator = new GenericTypeCreator();

        internal void InitializeMembersOf(T destinationObject)
        {
            var parameters = typeof(T).GetConstructors().ToList().SelectMany(ctorInfo => ctorInfo.GetParameters());

            foreach (var parameterInfo in parameters)
            {
                var parameterType = parameterInfo.ParameterType;
                if (parameterType == typeof(T)) continue;

                var defaultValue = CreateDefaultValueBasedOnParameterType(parameterInfo);

                if (defaultValue != null)
                    _memberSetter.SetMember(destinationObject, parameterInfo.Name, defaultValue);
            }
        }

        private object CreateDefaultValueBasedOnParameterType(ParameterInfo parameterInfo)
        {
            object defaultValue = null;
            var parameterType = parameterInfo.ParameterType;

            if (parameterType == typeof(string))
            {
                defaultValue = parameterInfo.Name;
            }
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterType))
            {
                defaultValue = _genericTypeCreator.CreateInstanceFor(parameterType.GenericTypeArguments);
            }
            else if (parameterType.IsClass && !parameterType.IsAbstract)
            {
                var typeOfBuilder = typeof(FluentBuilder<>).MakeGenericType(parameterType);
                var builderForChildObject = Activator.CreateInstance(typeOfBuilder);
                var methodInfo = typeOfBuilder.GetMethod("Build");
                defaultValue = methodInfo.Invoke(builderForChildObject, new object[] { });
            }
            return defaultValue;
        }
    }
}
