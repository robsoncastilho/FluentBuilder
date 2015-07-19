using System;

namespace Nosbor.FluentBuilder.Internals.Commands
{
    internal class SetStubCommand<TAbstraction, TReturn> : ICommand where TAbstraction : class
    {
        private object _object;
        private Func<TAbstraction, TReturn> _setupFunction;
        private TReturn _returnValue;

        internal SetStubCommand(object @object, Func<TAbstraction, TReturn> setupFunction, TReturn returnValue)
        {
            _object = @object;
            _setupFunction = setupFunction;
            _returnValue = returnValue;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
