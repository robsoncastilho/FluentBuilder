using Nosbor.FluentBuilder.Exceptions;
using System;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetPropertyCommand : BaseCommand, ICommand
    {
        private readonly object _object;
        private readonly object _newValue;
        private readonly PropertyInfo _propertyInfo;

        internal SetPropertyCommand(object @object, string propertyName, object newValue)
        {
            ValidateArguments(@object, propertyName, newValue);
            _propertyInfo = @object.GetType().GetProperty(propertyName);
            _object = @object;
            _newValue = newValue;
            ValidateProperty();
        }

        private void ValidateArguments(object @object, string propertyName, object newValue)
        {
            if (@object == null)
                throw new FluentBuilderException(AppendErrorMessage("Destination object is null"), new ArgumentNullException("object"));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new FluentBuilderException(AppendErrorMessage("Property name is null"), new ArgumentNullException("propertyName"));

            if (newValue == null)
                throw new FluentBuilderException(AppendErrorMessage("Value is null"), new ArgumentNullException("newValue"));
        }

        private void ValidateProperty()
        {
            if (_propertyInfo == null)
                throw new FluentBuilderException(AppendErrorMessage("Property not found"));

            if (!_propertyInfo.CanWrite)
                throw new FluentBuilderException(AppendErrorMessage("Property must have a setter"));

            if (!_propertyInfo.PropertyType.IsAssignableFrom(_newValue.GetType()))
                throw new FluentBuilderException(AppendErrorMessage("Value must be of the same type of the property"));
        }

        public void Execute()
        {
            _propertyInfo.SetValue(_object, _newValue, null);
        }
    }
}
