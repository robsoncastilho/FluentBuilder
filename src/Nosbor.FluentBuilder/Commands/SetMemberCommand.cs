using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Queries;
using System;
using System.Linq;
using System.Reflection;

namespace Nosbor.FluentBuilder.Commands
{
    internal class SetMemberCommand : ICommand
    {
        private readonly object _object;
        private readonly object _newValue;
        private readonly MemberInfo[] _membersInfo;
        private const BindingFlags DefaultMemberBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        private string _errorMessage = "Can't set value";

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
        }

        // TODO: refactor
        public void Execute()
        {
            ICommand command = null;
            foreach (var memberInfo in _membersInfo.ToList().OrderBy(m => m.MemberType))
            {
                var memberName = memberInfo.Name;
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    command = new SetFieldCommand(_object, memberName, _newValue);
                    break;
                }

                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo)memberInfo;
                    if (propertyInfo.CanWrite)
                        command = new SetPropertyCommand(_object, memberName, _newValue);
                    else
                    {
                        var fieldName = GetMemberQuery.GetFieldNameFor(_object, memberName);
                        command = new SetFieldCommand(_object, fieldName, _newValue);
                    }
                }
            }

            if (command == null) return;
            command.Execute();
        }

        private string AppendErrorMessage(string aditionalMessage)
        {
            return string.Format("{0} - {1}", _errorMessage, aditionalMessage);
        }
    }
}
