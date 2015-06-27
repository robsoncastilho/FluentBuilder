using System;
using System.Linq;

namespace Nosbor.FluentBuilder.Internals
{
    internal class ConstrutorMembersInitializer<T> where T : class
    {
        private MemberSetter<T> _memberSetter = new MemberSetter<T>();

        internal void InitializeMembersOf(T destinationObject)
        {
            var parameters = typeof(T).GetConstructors().ToList().SelectMany(ctorInfo => ctorInfo.GetParameters());

            foreach (var parameter in parameters)
            {
                object defaultValue = null;
                var parameterType = parameter.ParameterType;
                if (parameterType == typeof(string))
                {
                    defaultValue = parameter.Name;
                }
                else if (parameterType.IsClass && parameterType != typeof(T))
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
