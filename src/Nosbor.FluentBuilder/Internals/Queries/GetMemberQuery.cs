using Nosbor.FluentBuilder.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals.Queries
{
    internal static class GetMemberQuery
    {
        private const BindingFlags DefaultFieldBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        internal static string GetMemberNameFor<T, TProperty>(Expression<Func<T, TProperty>> expression) where T : class
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new FluentBuilderException(string.Format("Property missing in '{0}'", expression), new ArgumentException("Argument should be a MemberExpression", "expression"));

            if (memberExpression.Expression.ToString().Contains("."))
                throw new FluentBuilderException(string.Format("Nested property {0} not allowed", expression), new ArgumentException("Argument should be a direct property of the object being constructed", "expression"));

            return memberExpression.Member.Name;
        }

        internal static string GetFieldNameFor(object @object, string memberName)
        {
            var fieldInfo = GetFieldInfoFor(@object.GetType(), memberName);
            return (fieldInfo == null) ? string.Empty : fieldInfo.Name;
        }

        internal static FieldInfo GetFieldInfoFor(Type objectType, string memberName)
        {
            if (objectType.Name == "Object") return null;

            foreach (var fieldNameConvention in GetDefaultConventionsFor(memberName))
            {
                var fieldInfo = objectType.GetField(fieldNameConvention, DefaultFieldBindingFlags);
                if (fieldInfo != null) return fieldInfo;
            }

            return GetFieldInfoFor(objectType.BaseType, memberName);
        }

        private static IEnumerable<string> GetDefaultConventionsFor(string fieldName)
        {
            return new[] { fieldName, "_" + fieldName };
        }
    }
}
