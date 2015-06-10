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
        private readonly Dictionary<string, object> _members;
        private readonly Dictionary<string, object> _dependencies;

        private FluentBuilder()
        {
            _members = new Dictionary<string, object>();
            _dependencies = new Dictionary<string, object>();
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
            var newObject = (T)FormatterServices.GetUninitializedObject(typeof(T));

            try
            {
                foreach (var member in _members)
                {
                    SetProperty(newObject, member.Key, member.Value);
                }

                foreach (var dependency in _dependencies)
                {
                    SetField(newObject, dependency.Key, dependency.Value);
                }
                return newObject;
            }
            catch (Exception exception)
            {
                throw new FluentBuilderException(exception.Message, exception);
            }
        }

        /// <summary>
        /// Configures the builder to set the property with the informed value.
        /// </summary>
        public FluentBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> property, TProperty newValue)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new FluentBuilderException("A property is required in the expression", new ArgumentException("Argument should be a MemberExpression", "property"));

            var memberName = memberExpression.Member.Name;
            _members[memberName] = newValue;
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
        public FluentBuilder<T> Adding<TCollectionProperty, TElement>(Expression<Func<T, TCollectionProperty>> property, TElement newElement)
            where TCollectionProperty : ICollection<TElement>
        {
            throw new NotImplementedException();
        }

        private static void SetProperty(T newObject, string propertyName, object newValue)
        {
            typeof(T).GetProperty(propertyName).SetValue(newObject, newValue, null);
        }

        private static void SetField(T newObject, string baseName, object newValue)
        {
            // TODO: allow other conventions?
            var fieldName = "_" + Regex.Replace(baseName, "^[I]", "");

            typeof(T).GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).SetValue(newObject, newValue);
        }
    }
}