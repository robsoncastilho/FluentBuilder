using Nosbor.FluentBuilder.Exceptions;
using System;
using System.Linq;

namespace Nosbor.FluentBuilder.Internals
{
    internal class ConstrutorMembersInitializer
    {
        internal void InitializeMembersOf<T>(T destinationObject) where T : class
        {
            var parameters = typeof(T).GetConstructors().ToList().SelectMany(ctorInfo => ctorInfo.GetParameters());

            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;
                if (parameterType == typeof(string))
                {
                    var propertyName = ToPropertyConvention(parameter.Name);
                    SetWritableProperty(destinationObject, propertyName, propertyName);
                }
                else if (parameterType.IsClass && parameterType != typeof(T))
                {
                    var typeOfBuilder = typeof(FluentBuilder<>).MakeGenericType(parameterType);
                    var builderForChildObject = Activator.CreateInstance(typeOfBuilder);

                    var methodInfo = typeOfBuilder.GetMethod("Build");
                    var instanceOfChildObject = methodInfo.Invoke(builderForChildObject, new object[] { });

                    var propertyName = ToPropertyConvention(parameter.Name);
                    SetWritableProperty(destinationObject, propertyName, instanceOfChildObject);
                }
            }
        }

        private void SetWritableProperty<T>(T destinationObject, string propertyName, object newValue)
        {
            try
            {
                var propertyInfo = typeof(T).GetProperty(propertyName);
                propertyInfo.SetValue(destinationObject, newValue, null);
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(string.Format("Failed setting value for '{0}'", propertyName), exception);
            }
        }

        private string ToPropertyConvention(string name)
        {
            return name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
        }
    }
}
