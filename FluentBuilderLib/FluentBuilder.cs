using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace FluentBuilderLib
{
    public class FluentBuilder<T> where T : class
    {
        private readonly Dictionary<string, object> _propertiesToSet;

        private FluentBuilder()
        {
            _propertiesToSet = new Dictionary<string, object>();
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

            foreach (var keyValuePair in _propertiesToSet)
            {
                if (TryToSetProperty(newObject, keyValuePair.Key, keyValuePair.Value) ||
                    TryToSetField(newObject, keyValuePair.Key, keyValuePair.Value))
                    continue;

                throw new Exception(string.Format("Property/field {0} not found", keyValuePair.Key));
            }
            return newObject;
        }

        /// <summary>
        /// Configures the builder to set the property with the informed value.
        /// </summary>
        public FluentBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> property, object newValue)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Argument should be a MemberExpression", "property");

            var propertyName = memberExpression.Member.Name;
            _propertiesToSet[propertyName] = newValue;
            return this;
        }

        /// <summary>
        /// Sets the value for the property if the property is writable.
        /// </summary>
        private static bool TryToSetProperty(T newObject, string propertyName, object newValue)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null || !propertyInfo.CanWrite) return false;

            propertyInfo.SetValue(newObject, newValue, null);
            return true;
        }

        /// <summary>
        /// Sets the value for the field if the field corresponding to the property exists.
        /// </summary>
        private static bool TryToSetField(T newObject, string propertyName, object newValue)
        {
            var fieldName = "_" + propertyName.ToLower();
            var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null) return false;

            field.SetValue(newObject, newValue);
            return true;
        }

        #region PtBr

        public static FluentBuilder<T> Novo()
        {
            return New();
        }

        public T Criar()
        {
            return Build();
        }

        public FluentBuilder<T> Com<TPropriedade>(Expression<Func<T, TPropriedade>> propriedade, object novoValor)
        {
            return With(propriedade, novoValor);
        }

        #endregion
    }
}