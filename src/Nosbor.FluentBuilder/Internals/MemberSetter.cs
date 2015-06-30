using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nosbor.FluentBuilder.Internals
{
    internal class MemberSetter<T> where T : class
    {
        private const BindingFlags DefaultFieldBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
        private readonly GenericTypeCreator _genericTypeCreator = new GenericTypeCreator();

        internal void SetMember(T destinationObject, string memberName, object newValue)
        {
            var members = typeof(T).GetMember(memberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var memberInfo in members)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo)memberInfo).SetValue(destinationObject, newValue);
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo)memberInfo;
                    if (propertyInfo.CanWrite)
                        propertyInfo.SetValue(destinationObject, newValue);
                    else
                        SetField(destinationObject, memberName, newValue);
                }
            }
        }

        internal void SetField(T destinationObject, string fieldName, object newValue)
        {
            var fieldInfo = GetFieldApplyingConventionsIn(fieldName);
            fieldInfo.SetValue(destinationObject, newValue);
        }

        internal void SetWritableProperty(T destinationObject, string propertyName, object newValue)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            propertyInfo.SetValue(destinationObject, newValue, null);
        }

        internal void SetCollection(T destinationObject, string collectionName, IList<object> collectionValues)
        {
            var fieldInfo = GetFieldApplyingConventionsIn(collectionName);

            var genericListInstance = _genericTypeCreator.CreateInstanceFor(fieldInfo.FieldType.GenericTypeArguments);
            fieldInfo.SetValue(destinationObject, genericListInstance);

            AddElementsTo(destinationObject, fieldInfo, genericListInstance.GetType(), collectionValues);
        }

        private static void AddElementsTo(T destinationObject, FieldInfo fieldInfo, Type genericListType, IList<object> collectionValues)
        {
            var methodInfo = genericListType.GetMethod("Add");
            var fieldInstance = fieldInfo.GetValue(destinationObject);

            foreach (var value in collectionValues)
                methodInfo.Invoke(fieldInstance, new[] { value });
        }

        private static FieldInfo GetFieldApplyingConventionsIn(string fieldName)
        {
            FieldInfo fieldInfo = null;
            foreach (var fieldNameConvention in GetDefaultConventionsFor(fieldName))
            {
                fieldInfo = typeof(T).GetField(fieldNameConvention, DefaultFieldBindingFlags);
                if (fieldInfo != null) break;
            }
            return fieldInfo;
        }

        private static IEnumerable<string> GetDefaultConventionsFor(string fieldName)
        {
            return new[] { fieldName, "_" + fieldName };
        }
    }
}
