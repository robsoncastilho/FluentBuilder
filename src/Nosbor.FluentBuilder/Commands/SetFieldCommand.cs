using Nosbor.FluentBuilder.Exceptions;
using System;
using System.Reflection;

namespace Nosbor.FluentBuilder.Commands
{
    internal class SetFieldCommand : ICommand
    {
        private object _object;
        private object _newValue;
        private FieldInfo _fieldInfo;
        private string _errorMessage = "Can't set value";

        internal SetFieldCommand(object @object, string fieldName, object newValue)
        {
            ValidateArguments(@object, fieldName, newValue);
            _object = @object;
            _newValue = newValue;
            _fieldInfo = @object.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            ValidateField();
        }

        private void ValidateArguments(object @object, string fieldName, object newValue)
        {
            if (@object == null)
                throw new FluentBuilderException(AppendErrorMessage("Destination object is null"), new ArgumentNullException("@object"));

            if (string.IsNullOrWhiteSpace(fieldName))
                throw new FluentBuilderException(AppendErrorMessage("Field name is null"), new ArgumentNullException("fieldName"));

            if (newValue == null)
                throw new FluentBuilderException(AppendErrorMessage("Value is null"), new ArgumentNullException("newValue"));
        }

        private void ValidateField()
        {
            if (_fieldInfo == null)
                throw new FluentBuilderException(AppendErrorMessage("Field not found"));

            if (!_fieldInfo.FieldType.IsAssignableFrom(_newValue.GetType()))
                throw new FluentBuilderException(AppendErrorMessage("Value must be of the same type of the field"));
        }

        public void Execute()
        {
            _fieldInfo.SetValue(_object, _newValue);
        }

        private string AppendErrorMessage(string aditionalMessage)
        {
            return string.Format("{0} - {1}", _errorMessage, aditionalMessage);
        }
    }
}
