using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals.Support;
using System;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetDefaultValuesCommand : ICommand
    {
        private readonly object _object;
        private readonly BindingFlags _allowedBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public SetDefaultValuesCommand(object @object)
        {
            _object = @object;
        }

        public void Execute()
        {
            var objectType = _object.GetType();

            var fieldsToInitialize = objectType.GetMembers(_allowedBindingFlags)
                .Where(memberInfo => memberInfo.MemberType == MemberTypes.Field)
                .Select(memberInfo => memberInfo as FieldInfo)
                .Where(fieldInfo => fieldInfo.FieldType.IsAllowedToInitialize(objectType))
                .ToList();

            fieldsToInitialize.ForEach(fieldInfo => InitializeField(fieldInfo));
        }

        private void InitializeField(FieldInfo fieldInfo)
        {
            try
            {
                var defaultValueGenerator = fieldInfo.FieldType.GetDefaultValueGenerator();

                var defaultValue = defaultValueGenerator.GetDefaultValueFor(fieldInfo.FieldType);
                if (defaultValue == null) return;

                var command = new SetFieldCommand(_object, fieldInfo.Name, defaultValue);
                command.Execute();
            }
            catch (Exception ex)
            {
                throw new FluentBuilderException(string.Format("Failed setting default value for field \"{0}\" - Object \"{1}\"", fieldInfo.Name, _object.GetType()), ex);
            }
        }
    }
}
