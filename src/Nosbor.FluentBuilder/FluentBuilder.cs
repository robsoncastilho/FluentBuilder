using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Linq;

namespace Nosbor.FluentBuilder
{
    public sealed class FluentBuilder<T> where T : class
    {
        private T _newObject = (T)FormatterServices.GetUninitializedObject(typeof(T));

        private ConstrutorMembersInitializer<T> _membersInitializer = new ConstrutorMembersInitializer<T>();
        private MemberSetter<T> _memberSetter = new MemberSetter<T>();

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _dependencies = new Dictionary<string, object>();
        private readonly Dictionary<string, IList<object>> _collections = new Dictionary<string, IList<object>>();

        /// <summary>
        /// Returns an instance of the builder to start the fluent creation of the object.
        /// </summary>
        public static FluentBuilder<T> New()
        {
            return new FluentBuilder<T>();
        }

		/// <summary>
		/// Returns a list with the amount of requested items
		/// </summary>
		/// <param name="howMany">The amount of requested items</param>
		public static IList<T> Many(int howMany)
		{
			return Enumerable
				.Range(0, howMany)
				.Select(i => New().Build())
				.ToList();
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
            _membersInitializer.InitializeMembersOf(_newObject);
            SetAllMembers();
            return _newObject;
        }

        /// <summary>
        /// Configures the builder to set a property with the informed value.
        /// <para>(Property must not be read-only)</para>
        /// </summary>
        public FluentBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty newValue)
        {
            var propertyName = GetMemberNameFor(expression);
            _properties[propertyName] = newValue;
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
            var dependencyName = typeof(TServiceInterface).Name;
            _dependencies[dependencyName] = serviceImplementation;
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
            var propertyName = GetMemberNameFor(expression);

            if (!_collections.ContainsKey(propertyName))
                _collections[propertyName] = new List<object>();

            _collections[propertyName].Add(newElement);
            return this;
        }

        private string GetMemberNameFor<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new FluentBuilderException(string.Format("Property missing in '{0}'", expression), new ArgumentException("Argument should be a MemberExpression", "expression"));

            if (memberExpression.Expression.ToString().Contains("."))
                throw new FluentBuilderException(string.Format("Nested property {0} not allowed", expression), new ArgumentException("Argument should be a direct property of the object being constructed", "expression"));

            return memberExpression.Member.Name;
        }

        private void SetAllMembers()
        {
            foreach (var property in _properties)
                SetWritableProperty(property.Key, property.Value);

            foreach (var dependency in _dependencies)
                SetDependencyField(dependency.Key, dependency.Value);

            foreach (var collection in _collections)
                SetCollection(collection.Key, collection.Value);
        }

        private void SetWritableProperty(string propertyName, object newValue)
        {
            try
            {
                _memberSetter.SetWritableProperty(_newObject, propertyName, newValue);
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(string.Format("Failed setting value for '{0}'", propertyName), exception);
            }
        }

        private void SetDependencyField(string fieldName, object newValue)
        {
            try
            {
                _memberSetter.SetField(_newObject, Regex.Replace(fieldName, "^I", ""), newValue);
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(string.Format("Failed setting value for '{0}'", fieldName), exception);
            }
        }

        private void SetCollection(string collectionName, IList<object> collectionValues)
        {
            try
            {
                _memberSetter.SetCollection(_newObject, collectionName, collectionValues);
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(string.Format("Failed setting value for '{0}'", collectionName), exception);
            }
        }
    }
}