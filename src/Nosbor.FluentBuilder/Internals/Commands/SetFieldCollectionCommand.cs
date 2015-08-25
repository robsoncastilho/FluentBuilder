using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals.Queries;
using Nosbor.FluentBuilder.Internals.Support;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetFieldCollectionCommand : ICommand
    {
        private readonly object _object;
        private readonly string _collectionName;
        private readonly IList<object> _newValues = new List<object>();
        private readonly FieldInfo _fieldInfo;
        private readonly GenericTypeCreator _genericTypeCreator = new GenericTypeCreator();

        internal SetFieldCollectionCommand(object @object, string collectionName)
        {
            ValidateArguments(@object, collectionName);
            _object = @object;
            _collectionName = collectionName;
            _fieldInfo = GetMemberQuery.GetFieldInfoFor(_object.GetType(), _collectionName);
            ValidateField();
        }

        private readonly ICommand _command;

        internal SetFieldCollectionCommand(ICommand command)
        {
            _command = command;
        }

        private void ValidateArguments(object @object, string collectionName)
        {
            if (@object == null)
                throw new FluentBuilderException(AppendErrorMessage("Destination object is null"), new ArgumentNullException("object"));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new FluentBuilderException(AppendErrorMessage("Collection name is null"), new ArgumentNullException("collectionName"));
        }

        private void ValidateField()
        {
            if (_fieldInfo == null)
                throw new FluentBuilderException(AppendErrorMessage("Field not found"));
        }

        private string _errorMessage = "Can't set value";

        private string AppendErrorMessage(string aditionalMessage)
        {
            return string.Format("{0} - {1}", _errorMessage, aditionalMessage);
        }

        internal void Add(object newValue)
        {
            _newValues.Add(newValue);
        }

        public void Execute()
        {
            try
            {
                var genericListInstance = _genericTypeCreator.CreateInstanceFor(typeof(List<>), _fieldInfo.FieldType.GenericTypeArguments);

                var command = new SetFieldCommand(_object, _collectionName, genericListInstance);
                command.Execute();

                AddElementsTo(genericListInstance.GetType());
            }
            catch (Exception ex)
            {
                throw new FluentBuilderException(string.Format("Failed setting collection \"{0}\" - Object \"{1}\"", _collectionName, _object.GetType()), ex);
            }
        }

        private void AddElementsTo(Type genericListType)
        {
            var methodInfo = genericListType.GetMethod("Add");
            var fieldInstance = _fieldInfo.GetValue(_object);

            foreach (var value in _newValues)
                methodInfo.Invoke(fieldInstance, new[] { value });
        }
    }
}
