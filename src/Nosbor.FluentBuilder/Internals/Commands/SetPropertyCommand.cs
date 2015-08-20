using Nosbor.FluentBuilder.Exceptions;
using System.Reflection;
using System.Text;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetPropertyCommand : BaseCommand, ICommand
    {
        private readonly PropertyInfo _propertyInfo;

        internal SetPropertyCommand(object destinationObject, string propertyName, object newValue) : base(destinationObject, propertyName, newValue)
        {
            _propertyInfo = destinationObject.GetType().GetProperty(propertyName);
            ValidateProperty();
        }

        private void ValidateProperty()
        {
            if (_propertyInfo == null)
                throw new FluentBuilderException(string.Format("Property \"{0}\" not found - Object \"{1}\"", MemberName, DestinationObject));

            if (!_propertyInfo.CanWrite)
                throw new FluentBuilderException(string.Format("Property \"{0}\" must have a setter - Object \"{1}\"", MemberName, DestinationObject));

            if (!_propertyInfo.PropertyType.IsAssignableFrom(MemberNewValue.GetType()))
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendFormat("Value must be of the same type of the property \"{0}\" - Object \"{1}\"\n", MemberName, DestinationObject);
                messageBuilder.AppendFormat("Informed type: {0}\n", MemberNewValue.GetType());
                messageBuilder.AppendFormat("Property type: {0}", _propertyInfo.PropertyType);
                throw new FluentBuilderException(messageBuilder.ToString());
            }
        }

        public void Execute()
        {
            _propertyInfo.SetValue(DestinationObject, MemberNewValue, null);
        }
    }
}
