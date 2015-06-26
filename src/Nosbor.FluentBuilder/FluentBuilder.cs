using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nosbor.FluentBuilder
{
    public sealed class FluentBuilder<T> where T : class
    {
        private T _newObject = (T)FormatterServices.GetUninitializedObject(typeof(T));

        private ConstrutorMembersInitializer _membersInitializer = new ConstrutorMembersInitializer();

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _dependencies = new Dictionary<string, object>();
        private readonly Dictionary<string, IList<object>> _collections = new Dictionary<string, IList<object>>();

        private static BindingFlags _defaultFieldBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance |
                                                                BindingFlags.Static | BindingFlags.NonPublic;

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
            InitializeRequiredMembers();
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

        private static string[] GetDefaultConventionsFor(string fieldName)
        {
            return new[] { fieldName, "_" + fieldName };
        }

        private static FieldInfo GetFieldApplyingConventionsIn(string fieldName)
        {
            FieldInfo fieldInfo = null;
            foreach (var fieldNameConvention in GetDefaultConventionsFor(fieldName))
            {
                fieldInfo = typeof(T).GetField(fieldNameConvention, _defaultFieldBindingFlags);
                if (fieldInfo != null) break;
            }
            return fieldInfo;
        }

        private void InitializeRequiredMembers()
        {
            _membersInitializer.InitializeMembersOf(_newObject);
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
                var propertyInfo = typeof(T).GetProperty(propertyName);
                propertyInfo.SetValue(_newObject, newValue, null);
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
                var fieldInfo = GetFieldApplyingConventionsIn(Regex.Replace(fieldName, "^I", ""));
                fieldInfo.SetValue(_newObject, newValue);
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
                var fieldInfo = GetFieldApplyingConventionsIn(collectionName);
                var genericList = BuildGenericListFrom(fieldInfo);
                SetGenericListInstanceTo(fieldInfo, genericList);
                AddElementsTo(fieldInfo, genericList, collectionValues);
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(string.Format("Failed setting value for '{0}'", collectionName), exception);
            }
        }

        private Type BuildGenericListFrom(FieldInfo fieldInfo)
        {
            var genericTypes = fieldInfo.FieldType.GenericTypeArguments;
            return typeof(List<>).MakeGenericType(genericTypes);
        }

        private void SetGenericListInstanceTo(FieldInfo fieldInfo, Type genericListType)
        {
            var instance = Activator.CreateInstance(genericListType);
            fieldInfo.SetValue(_newObject, instance);
        }

        private void AddElementsTo(FieldInfo fieldInfo, Type genericListType, IList<object> collectionValues)
        {
            var methodInfo = genericListType.GetMethod("Add");
            var fieldInstance = fieldInfo.GetValue(_newObject);

            foreach (var value in collectionValues)
                methodInfo.Invoke(fieldInstance, new object[] { value });
        }
    }
}