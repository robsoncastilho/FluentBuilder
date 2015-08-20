using Nosbor.FluentBuilder.Internals.DefaultValueGenerators;
using Nosbor.FluentBuilder.Internals.Support;
using System;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetDefaultValuesCommand : ICommand
    {
        private readonly object _object;
        private readonly IDefaultValueGeneratorFactory _defaultValueGeneratorFactory;
        private readonly BindingFlags _allowedBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public SetDefaultValuesCommand(object @object, IDefaultValueGeneratorFactory defaultValueGeneratorFactory)
        {
            _object = @object;
            _defaultValueGeneratorFactory = defaultValueGeneratorFactory;
        }

        public void Execute()
        {
            var objectType = _object.GetType();

            var fieldsToInitialize = objectType.GetMembers(_allowedBindingFlags)
                .Where(memberInfo => memberInfo.MemberType == MemberTypes.Field)
                .Select(memberInfo => memberInfo as FieldInfo)
                .Where(fieldInfo => IsAllowedToInitialize(fieldInfo.FieldType, objectType))
                .ToList();

            fieldsToInitialize.ForEach(fieldInfo => InitializeField(fieldInfo));
        }

        private bool IsAllowedToInitialize(Type fieldType, Type objectType)
        {
            return fieldType != objectType &&
                   (fieldType.IsString() || fieldType.IsConcreteClass() || fieldType.InheritsFrom<System.Collections.IEnumerable>());
        }

        private void InitializeField(FieldInfo fieldInfo)
        {
            var defaultValueGenerator = _defaultValueGeneratorFactory.CreateFor(fieldInfo.FieldType);

            var defaultValue = defaultValueGenerator.GetDefaultValueFor(fieldInfo.FieldType);
            if (defaultValue == null) return;

            var command = new SetFieldCommand(_object, fieldInfo.Name, defaultValue);
            command.Execute();
        }
    }
}
