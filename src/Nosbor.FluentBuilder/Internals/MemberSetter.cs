
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Nosbor.FluentBuilder.Internals
{
    internal class MemberSetter<T> where T : class
    {
        private const BindingFlags DefaultFieldBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

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
            var genericList = BuildGenericListFrom(fieldInfo);
            SetGenericListInstanceTo(destinationObject, fieldInfo, genericList);
            AddElementsTo(destinationObject, fieldInfo, genericList, collectionValues);
        }

        private static Type BuildGenericListFrom(FieldInfo fieldInfo)
        {
            var genericTypes = fieldInfo.FieldType.GenericTypeArguments;
            return typeof(List<>).MakeGenericType(genericTypes);
        }

        private static void SetGenericListInstanceTo(T destinationObject, FieldInfo fieldInfo, Type genericListType)
        {
            var instance = Activator.CreateInstance(genericListType);
            fieldInfo.SetValue(destinationObject, instance);
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
