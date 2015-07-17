using Nosbor.FluentBuilder.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Queries
{
    internal static class GetMemberQuery
    {
        private const BindingFlags DefaultFieldBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

        internal static string GetFieldNameFor<T>(string memberName) where T : class
        {
            return GetFieldNameFor(typeof(T), memberName);
        }

        internal static string GetFieldNameFor(object @object, string memberName)
        {
            return GetFieldNameFor(@object.GetType(), memberName);
        }

        internal static string GetFieldNameFor<T, TProperty>(Expression<Func<T, TProperty>> expression) where T : class
        {
            var memberName = GetMemberNameFor<T, TProperty>(expression);
            return GetFieldNameFor<T>(memberName);
        }

        internal static string GetPropertyNameFor<T, TProperty>(Expression<Func<T, TProperty>> expression) where T : class
        {
            return GetMemberNameFor<T, TProperty>(expression);
        }

        private static string GetFieldNameFor(Type objectType, string memberName)
        {
            FieldInfo fieldInfo = null;
            foreach (var fieldNameConvention in GetDefaultConventionsFor(memberName))
            {
                fieldInfo = objectType.GetField(fieldNameConvention, DefaultFieldBindingFlags);
                if (fieldInfo != null) break;
            }
            return (fieldInfo == null) ? string.Empty : fieldInfo.Name;
        }

        private static IEnumerable<string> GetDefaultConventionsFor(string fieldName)
        {
            return new[] { fieldName, "_" + fieldName };
        }

        private static string GetMemberNameFor<T, TProperty>(Expression<Func<T, TProperty>> expression) where T : class
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new FluentBuilderException(string.Format("Property missing in '{0}'", expression), new ArgumentException("Argument should be a MemberExpression", "expression"));

            if (memberExpression.Expression.ToString().Contains("."))
                throw new FluentBuilderException(string.Format("Nested property {0} not allowed", expression), new ArgumentException("Argument should be a direct property of the object being constructed", "expression"));

            return memberExpression.Member.Name;
        }
    }
}
