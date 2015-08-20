using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals.Queries;
using System.Reflection;
using System.Text;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetFieldCommand : BaseCommand, ICommand
    {
        private readonly FieldInfo _fieldInfo;

        internal SetFieldCommand(object destinationObject, string fieldName, object newValue) : base(destinationObject, fieldName, newValue)
        {
            _fieldInfo = GetMemberQuery.GetFieldInfoFor(destinationObject.GetType(), fieldName);
            ValidateField();
        }

        private void ValidateField()
        {
            if (_fieldInfo == null)
                throw new FluentBuilderException(string.Format("Field \"{0}\" not found - Object \"{1}\"", MemberName, DestinationObject));

            if (!_fieldInfo.FieldType.IsAssignableFrom(MemberNewValue.GetType()))
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendFormat("Value must be of the same type of the field \"{0}\" - Object \"{1}\"\n", MemberName, DestinationObject);
                messageBuilder.AppendFormat("Informed type: {0}\n", MemberNewValue.GetType());
                messageBuilder.AppendFormat("Field type: {0}", _fieldInfo.FieldType);
                throw new FluentBuilderException(messageBuilder.ToString());
            }
        }

        public void Execute()
        {
            _fieldInfo.SetValue(DestinationObject, MemberNewValue);
        }
    }
}
