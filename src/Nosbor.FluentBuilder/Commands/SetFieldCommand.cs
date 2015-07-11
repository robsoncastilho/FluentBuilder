using System.Reflection;

namespace Nosbor.FluentBuilder.Commands
{
    internal class SetFieldCommand : ICommand
    {
        private object _object;
        private string _fieldName;
        private object _newValue;
        private FieldInfo _fieldInfo;

        internal SetFieldCommand(object @object, string fieldName, object newValue)
        {
            _object = @object;
            _fieldName = fieldName;
            _newValue = newValue;
            _fieldInfo = @object.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public void Execute()
        {
            _fieldInfo.SetValue(_object, _newValue);
        }
    }
}
