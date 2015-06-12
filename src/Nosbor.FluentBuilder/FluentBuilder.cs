using Nosbor.FluentBuilder.Exceptions;
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
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _dependencies = new Dictionary<string, object>();
        private readonly Dictionary<string, IList<object>> _collections = new Dictionary<string, IList<object>>();

        private static BindingFlags _defaultFieldBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance |
                                                                BindingFlags.Static | BindingFlags.NonPublic;

        private FluentBuilder() { }

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
            var newObject = (T)FormatterServices.GetUninitializedObject(typeof(T));
            SetAllMembersFor(newObject);
            return newObject;
        }

        /// <summary>
        /// Configures the builder to set the property with the informed value.
        /// </summary>
        public FluentBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty newValue)
        {
            var propertyName = GetMemberNameFor(expression);
            _properties[propertyName] = newValue;
            return this;
        }

        /// <summary>
        /// Configures the builder to set the field with the concrete service informed.
        /// This method makes possible to set a test double object as the service for testing purposes.
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
        /// </summary>
        public FluentBuilder<T> AddingTo<TCollectionProperty, TElement>(Expression<Func<T, TCollectionProperty>> expression, TElement newElement)
            where TCollectionProperty : IEnumerable<TElement>
        {
            var propertyName = GetMemberNameFor(expression);

            if (!_collections.ContainsKey(propertyName))
            {
                _collections[propertyName] = new List<object>();
            }

            _collections[propertyName].Add(newElement);
            return this;
        }

        private void SetAllMembersFor(T newObject)
        {
            foreach (var property in _properties)
            {
                SetWritableProperty(newObject, property.Key, property.Value);
            }

            foreach (var dependency in _dependencies)
            {
                SetDependencyField(newObject, dependency.Key, dependency.Value);
            }

            foreach (var collection in _collections)
            {
                SetCollection(newObject, collection.Key, collection.Value);
            }
        }

        private string GetMemberNameFor<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new FluentBuilderException(string.Format("Property missing in '{0}'", expression), new ArgumentException("Argument should be a MemberExpression", "expression"));

            return memberExpression.Member.Name;
        }

        private static void SetWritableProperty(T newObject, string propertyName, object newValue)
        {
            try
            {
                typeof(T).GetProperty(propertyName).SetValue(newObject, newValue, null);
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(string.Format("Failed setting value for '{0}'", propertyName), exception);
            }
        }

        private static void SetDependencyField(T newObject, string fieldName, object newValue)
        {
            try
            {
                var fieldInfo = GetFieldApplyingConventionsIn(Regex.Replace(fieldName, "^I", ""));
                fieldInfo.SetValue(newObject, newValue);
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(string.Format("Failed setting value for '{0}'", fieldName), exception);
            }
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

        private static string[] GetDefaultConventionsFor(string fieldName)
        {
            return new[] { fieldName, "_" + fieldName };
        }

        private void SetCollection(T newObject, string collectionName, IList<object> collectionValues)
        {

            try
            {
                var fieldInfo = GetFieldApplyingConventionsIn(collectionName);
                var genericList = BuildGenericListFrom(fieldInfo);
                SetGenericListInstanceTo(fieldInfo, genericList, newObject);
                AddElementsTo(fieldInfo, genericList, collectionValues, newObject);
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

        private void SetGenericListInstanceTo(FieldInfo fieldInfo, Type genericListType, T newObject)
        {
            var instance = Activator.CreateInstance(genericListType);
            fieldInfo.SetValue(newObject, instance);
        }

        private void AddElementsTo(FieldInfo fieldInfo, Type genericListType, IList<object> collectionValues, T newObject)
        {
            var methodInfo = genericListType.GetMethod("Add");
            var fieldInstance = fieldInfo.GetValue(newObject);

            foreach (var value in collectionValues)
                methodInfo.Invoke(fieldInstance, new object[] { value });
        }
    }
}