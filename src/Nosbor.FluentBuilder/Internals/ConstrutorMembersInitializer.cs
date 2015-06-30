using Nosbor.FluentBuilder.Lib;
using System;
using System.Linq;

namespace Nosbor.FluentBuilder.Internals
{
    internal class ConstrutorMembersInitializer<T> where T : class
    {
        private readonly MemberSetter<T> _memberSetter = new MemberSetter<T>();
        private readonly GenericTypeCreator _genericTypeCreator = new GenericTypeCreator();

        internal void InitializeMembersOf(T destinationObject)
        {
            var parameters = typeof(T).GetConstructors().ToList().SelectMany(ctorInfo => ctorInfo.GetParameters());

            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;
                if (parameterType == typeof(T)) continue;

                object defaultValue = null;
                if (parameterType == typeof(string))
                {
                    defaultValue = parameter.Name;
                }
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterType))
                {
                    defaultValue = _genericTypeCreator.CreateInstanceFor(parameterType.GenericTypeArguments);
                }
                else if (parameterType.IsClass)
                {
                    var typeOfBuilder = typeof(FluentBuilder<>).MakeGenericType(parameterType);
                    var builderForChildObject = Activator.CreateInstance(typeOfBuilder);
                    var methodInfo = typeOfBuilder.GetMethod("Build");
                    defaultValue = methodInfo.Invoke(builderForChildObject, new object[] { });
                }

                if (defaultValue != null)
                    _memberSetter.SetMember(destinationObject, parameter.Name, defaultValue);
            }
        }
    }
}
