using Nosbor.FluentBuilder.Internals.Commands;
using Nosbor.FluentBuilder.Internals.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Nosbor.FluentBuilder.Internals.Support;

namespace Nosbor.FluentBuilder.Lib
{
    public sealed class FluentBuilder<T> where T : class
    {
        private readonly T _newObject;
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        internal Dictionary<string, ICommand> Commands => _commands;

        public FluentBuilder()
        {
            _newObject = typeof(T).CreateInstance<T>();
            _commands["defaultValues"] = new SetDefaultValuesCommand(_newObject);
        }

        /// <summary>
        /// Returns an instance of the builder to start the fluent creation of the object.
        /// </summary>
        public static FluentBuilder<T> New()
        {
            return new FluentBuilder<T>();
        }

        /// <summary>
        /// Performs implicit conversion from builder to destination object so that it's not needed to call Build method explicitly.
        /// </summary>
        public static implicit operator T(FluentBuilder<T> builder)
        {
            return builder.Build();
        }

        /// <summary>
        /// Builds, sets all configurated properties and returns the destination object.
        /// </summary>
        public T Build()
        {
            foreach (var command in _commands.Values) command.Execute();
            return _newObject;
        }

        /// <summary>
        /// Like <see cref="Build()"/>, however it returns the destination object within a collection.
        /// </summary>
        public IEnumerable<T> AsList()
        {
            return new List<T> { Build() };
        }

        /// <summary>
        /// Returns a list with the amount of requested items
        /// </summary>        
        public static IEnumerable<T> Many(int howMany)
        {
            return Enumerable.Repeat(New().Build(), howMany);
        }

        /// <summary>
        /// Configures the builder to set a property with the informed value.
        /// </summary>
        public FluentBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty newValue)
        {
            var propertyName = GetMemberQuery.GetMemberNameFor(expression);
            _commands[propertyName] = new SetPropertyCommand(_newObject, propertyName, newValue);
            return this;
        }

        /// <summary>
        /// Configures the builder to set a collection passing all elements at once
        /// </summary>
        public FluentBuilder<T> WithCollection<TCollectionProperty, TElement>(Expression<Func<T, TCollectionProperty>> expression, params TElement[] newElements)
            where TCollectionProperty : IEnumerable<TElement>
        {
            var fieldName = GetMemberQuery.GetMemberNameFor(expression);

            var cmd = new SetFieldCollectionCommand(_newObject, fieldName);
            foreach (var element in newElements)
                cmd.Add(element);

            _commands[fieldName] = cmd;
            return this;
        }

        /// <summary>
        /// Configures the builder to set a field with the informed value.
        /// </summary>
        public FluentBuilder<T> WithFieldValue<TField>(TField newValue) where TField : class
        {
            var fieldName = typeof(TField).Name;
            _commands[fieldName] = new SetFieldCommand(_newObject, fieldName, newValue);
            return this;
        }

        /// <summary>
        /// Configures the builder to set a private field with the dependency informed.
        /// </summary>
        public FluentBuilder<T> WithDependency<TServiceInterface, TServiceImplementation>(TServiceImplementation serviceImplementation)
            where TServiceImplementation : TServiceInterface
        {
            var fieldName = Regex.Replace(typeof(TServiceInterface).Name, "^I", "");
            _commands[fieldName] = new SetFieldCommand(_newObject, fieldName, serviceImplementation);
            return this;
        }

        /// <summary>
        /// Configures the builder to add an element to a collection one by one.
        /// </summary>
        public FluentBuilder<T> AddingTo<TCollectionProperty, TElement>(Expression<Func<T, TCollectionProperty>> expression, TElement newElement)
            where TCollectionProperty : IEnumerable<TElement>
        {
            var fieldName = GetMemberQuery.GetMemberNameFor(expression);

            var setFieldCollectionCommand = GetCommandFor(fieldName);
            setFieldCollectionCommand.Add(newElement);
            return this;
        }
        
        private SetFieldCollectionCommand GetCommandFor(string fieldName)
        {
            if (_commands.ContainsKey(fieldName))
                return (SetFieldCollectionCommand)_commands[fieldName];

            var setFieldCollectionCommand = new SetFieldCollectionCommand(_newObject, fieldName);
            _commands[fieldName] = setFieldCollectionCommand;
            return setFieldCollectionCommand;
        }
    }
}