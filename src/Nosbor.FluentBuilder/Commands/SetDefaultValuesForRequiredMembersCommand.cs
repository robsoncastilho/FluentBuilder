using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals;
using Nosbor.FluentBuilder.Lib;
using System;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Commands
{
    internal class SetDefaultValuesForRequiredMembersCommand : BaseCommand, ICommand
    {
        private readonly object _object;
        private readonly GenericTypeCreator _genericTypeCreator = new GenericTypeCreator();

        internal SetDefaultValuesForRequiredMembersCommand(object @object)
        {
            ValidateArguments(@object);
            _object = @object;
        }

        private void ValidateArguments(object @object)
        {
            if (@object == null)
                throw new FluentBuilderException(AppendErrorMessage("Destination object is null"), new ArgumentNullException("@object"));
        }

        public void Execute()
        {
            var objectType = _object.GetType();
            var parameters = objectType.GetConstructors().ToList().SelectMany(ctorInfo => ctorInfo.GetParameters());

            foreach (var parameterInfo in parameters)
            {
                var parameterType = parameterInfo.ParameterType;
                if (parameterType == objectType) continue;

                var defaultValue = CreateDefaultValueBasedOnParameterType(parameterInfo);

                if (defaultValue != null)
                {
                    var command = new SetMemberCommand(_object, parameterInfo.Name, defaultValue);
                    command.Execute();
                }
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
