using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetDefaultValuesCommand : ICommand
    {
        private readonly object _object;
        private readonly List<FieldInfo> _fieldsToInitialize = new List<FieldInfo>();
        private const BindingFlags AllowedBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public SetDefaultValuesCommand(object @object)
        {
            _object = @object;
        }

        public void Execute()
        {
            var objectType = _object.GetType();

            AddFieldsToInitialize(objectType);

            _fieldsToInitialize.ForEach(InitializeField);
        }

        private void AddFieldsToInitialize(Type objectType)
        {
            if (objectType.Name == "Object") return;

            _fieldsToInitialize.AddRange(objectType.GetMembers(AllowedBindingFlags)
                .Where(memberInfo => memberInfo.MemberType == MemberTypes.Field)
                .Select(memberInfo => memberInfo as FieldInfo)
                .Where(fieldInfo => fieldInfo.FieldType.IsAllowedToInitialize(objectType)));
            
            AddFieldsToInitialize(objectType.BaseType());
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
