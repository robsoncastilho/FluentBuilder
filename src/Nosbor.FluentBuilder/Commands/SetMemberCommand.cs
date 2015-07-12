using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Queries;
using System;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Commands
{
    internal class SetMemberCommand : BaseCommand, ICommand
    {
        private readonly object _object;
        private readonly object _newValue;
        private readonly MemberInfo[] _membersInfo;
        private const BindingFlags DefaultMemberBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        internal SetMemberCommand(object @object, string memberName, object newValue)
        {
            ValidateArguments(@object, memberName, newValue);
            _object = @object;
            _newValue = newValue;
            _membersInfo = @object.GetType().GetMember(memberName, DefaultMemberBindingFlags);
            ValidateMembers();
        }

        private void ValidateArguments(object @object, string memberName, object newValue)
        {
            if (@object == null)
                throw new FluentBuilderException(AppendErrorMessage("Destination object is null"), new ArgumentNullException("@object"));

            if (string.IsNullOrWhiteSpace(memberName))
                throw new FluentBuilderException(AppendErrorMessage("Member name is null"), new ArgumentNullException("memberName"));

            if (newValue == null)
                throw new FluentBuilderException(AppendErrorMessage("Value is null"), new ArgumentNullException("newValue"));
        }

        private void ValidateMembers()
        {
            if (_membersInfo.Length == 0)
                throw new FluentBuilderException(AppendErrorMessage("Member not found"));

            if (_membersInfo.Any(memberInfo => memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property))
                throw new FluentBuilderException(AppendErrorMessage("Member must be a property or a field"));
        }

        public void Execute()
        {
            ICommand command = CreateCommandForField();
            if (command == null)
                command = CreateCommandForProperty();

            command.Execute();
        }

        private ICommand CreateCommandForField()
        {
            var memberInfo = _membersInfo.FirstOrDefault(mInfo => mInfo.MemberType == MemberTypes.Field);
            return (memberInfo == null) ? null : new SetFieldCommand(_object, memberInfo.Name, _newValue);
        }

        private ICommand CreateCommandForProperty()
        {
            var propertyInfo = (PropertyInfo)_membersInfo.FirstOrDefault(mInfo => mInfo.MemberType == MemberTypes.Property);
            if (propertyInfo.CanWrite)
                return new SetPropertyCommand(_object, propertyInfo.Name, _newValue);

            var fieldName = GetMemberQuery.GetFieldNameFor(_object, propertyInfo.Name);
            return new SetFieldCommand(_object, fieldName, _newValue);
        }
    }
}
