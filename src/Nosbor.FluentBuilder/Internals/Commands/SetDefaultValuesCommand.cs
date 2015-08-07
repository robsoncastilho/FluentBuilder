using Nosbor.FluentBuilder.Internals.DefaultValueGenerators;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetDefaultValuesCommand : BaseCommand, ICommand
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
                .Where(fieldInfo => fieldInfo.FieldType != objectType && (fieldInfo.FieldType == typeof(string) ||
                                    (fieldInfo.FieldType.IsClass && !fieldInfo.FieldType.IsAbstract) ||
                                    (typeof(System.Collections.IEnumerable).IsAssignableFrom(fieldInfo.FieldType))))
                .ToList();

            foreach (var memberToInitialize in fieldsToInitialize)
            {
                var defaultValueGenerator = _defaultValueGeneratorFactory.CreateFor(memberToInitialize.FieldType);

                var defaultValue = defaultValueGenerator.GetDefaultValueFor(memberToInitialize.FieldType);

                if (defaultValue != null)
                {
                    var command = new SetMemberCommand(_object, memberToInitialize.Name, defaultValue);
                    command.Execute();
                }
            }
        }
    }
}
