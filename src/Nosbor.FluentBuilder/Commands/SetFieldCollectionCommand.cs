using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nosbor.FluentBuilder.Commands
{
    internal class SetFieldCollectionCommand : ICommand
    {
        private object _object;
        private string _collectionName;
        private IList<object> _newValues = new List<object>();
        private FieldInfo _fieldInfo;
        private string _errorMessage = "Can't set value";

        private readonly GenericTypeCreator _genericTypeCreator = new GenericTypeCreator();

        public SetFieldCollectionCommand(object @object, string collectionName)
        {
            ValidateArguments(@object, collectionName);
            _object = @object;
            _collectionName = collectionName;
            _fieldInfo = _object.GetType().GetField(_collectionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private void ValidateArguments(object @object, string collectionName)
        {
            if (@object == null)
                throw new FluentBuilderException(AppendErrorMessage("Destination object is null"), new ArgumentNullException("@object"));

            if (collectionName == null)
                throw new FluentBuilderException(AppendErrorMessage("Collection name is null"), new ArgumentNullException("collectionName"));
        }

        internal void Add(object newValue)
        {
            _newValues.Add(newValue);
        }

        public void Execute()
        {
            var genericListInstance = _genericTypeCreator.CreateInstanceFor(_fieldInfo.FieldType.GenericTypeArguments);

            var command = new SetFieldCommand(_object, _collectionName, genericListInstance);
            command.Execute();

            AddElementsTo(genericListInstance.GetType());
        }

        private void AddElementsTo(Type genericListType)
        {
            var methodInfo = genericListType.GetMethod("Add");
            var fieldInstance = _fieldInfo.GetValue(_object);

            foreach (var value in _newValues)
                methodInfo.Invoke(fieldInstance, new[] { value });
        }

        private string AppendErrorMessage(string aditionalMessage)
        {
            return string.Format("{0} - {1}", _errorMessage, aditionalMessage);
        }
    }
}
