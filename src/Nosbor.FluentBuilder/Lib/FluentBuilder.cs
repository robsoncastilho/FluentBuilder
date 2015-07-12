using Nosbor.FluentBuilder.Commands;
using Nosbor.FluentBuilder.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nosbor.FluentBuilder.Lib
{
    public sealed class FluentBuilder<T> where T : class
    {
        private readonly T _newObject;
        private SetFieldCollectionCommand _setFieldCollectionCommand;
        private List<ICommand> _commands = new List<ICommand>();

        public FluentBuilder()
        {
            _newObject = (T)FormatterServices.GetUninitializedObject(typeof(T));
            _commands.Add(new SetDefaultValuesForRequiredMembersCommand(_newObject));
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
            _commands.ForEach(command => command.Execute());
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
            return Enumerable.Range(0, howMany).Select(i => New().Build()).ToList();
        }

        /// <summary>
        /// Configures the builder to set a property with the informed value.
        /// <para>(Property must not be read-only)</para>
        /// </summary>
        public FluentBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty newValue)
        {
            var propertyName = GetMemberQuery.GetPropertyNameFor<T, TProperty>(expression);
            _commands.Add(new SetPropertyCommand(_newObject, propertyName, newValue));
            return this;
        }

        /// <summary>
        /// Configures the builder to set a member with the informed value.
        /// <para>(Member name must be the same as its type name)</para>
        /// </summary>
        public FluentBuilder<T> With<TMember>(TMember newValue) where TMember : class
        {
            var memberName = typeof(TMember).Name;
            _commands.Add(new SetMemberCommand(_newObject, memberName, newValue));
            return this;
        }

        /// <summary>
        /// Configures the builder to set a private field with the dependency informed.
        /// <para>(This method makes possible to set a concrete dependency for integrated tests or a test double for unit tests)</para>
        /// <para>(If dependency interface is 'ICustomerRepository' then field name must be 'customerRepository' or '_customerRepository' - case is ignored)</para>
        /// </summary>
        public FluentBuilder<T> WithDependency<TServiceInterface, TServiceImplementation>(TServiceImplementation serviceImplementation)
            where TServiceImplementation : TServiceInterface
        {
            var fieldName = GetMemberQuery.GetFieldNameFor<T>(Regex.Replace(typeof(TServiceInterface).Name, "^I", ""));
            _commands.Add(new SetFieldCommand(_newObject, fieldName, serviceImplementation));
            return this;
        }

        /// <summary>
        /// Configures the builder to add an element to a collection.
        /// <para>(Property must have a corresponding private field)</para>
        /// <para>(If property name is 'Addresses' then field name must be 'addresses' or '_addresses')</para>
        /// </summary>
        public FluentBuilder<T> AddingTo<TCollectionProperty, TElement>(Expression<Func<T, TCollectionProperty>> expression, TElement newElement)
            where TCollectionProperty : IEnumerable<TElement>
        {
            var fieldName = GetMemberQuery.GetFieldNameFor<T, TCollectionProperty>(expression);

            if (_setFieldCollectionCommand == null)
            {
                _setFieldCollectionCommand = new SetFieldCollectionCommand(_newObject, fieldName);
                _commands.Add(_setFieldCollectionCommand);
            }
            _setFieldCollectionCommand.Add(newElement);
            return this;
        }
    }
}