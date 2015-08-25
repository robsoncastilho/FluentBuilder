using Nosbor.FluentBuilder.Exceptions;
using System;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal abstract class BaseCommand
    {
        protected readonly object DestinationObject;
        protected readonly string MemberName;
        protected readonly object MemberNewValue;

        protected BaseCommand(object destinationObject, string memberName, object memberNewValue)
        {
            ValidateArguments(destinationObject, memberName, memberNewValue);
            DestinationObject = destinationObject;
            MemberName = memberName;
            MemberNewValue = memberNewValue;
        }

        private void ValidateArguments(object destinationObject, string memberName, object memberNewValue)
        {
            if (destinationObject == null)
                throw new FluentBuilderException("Destination object is null", new ArgumentNullException("destinationObject"));

            if (string.IsNullOrWhiteSpace(memberName))
                throw new FluentBuilderException(string.Format("Member name required - Object \"{0}\"", destinationObject.GetType()),
                    new ArgumentNullException("memberName"));

            if (memberNewValue == null)
                throw new FluentBuilderException(string.Format("New value required - Object \"{0}\" - Member \"{1}\"",
                    destinationObject.GetType(), memberName), new ArgumentNullException("memberNewValue"));
        }
    }
}
