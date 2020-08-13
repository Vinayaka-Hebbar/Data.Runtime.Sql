using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SqlDb.Data
{
    /// <summary>
    /// Reflection Extension Methods
    /// </summary>
    public static class ReflectionHelper
    {
        public static IEnumerable<string> GetDataMemberNames(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (TryGetColumnName(property, out string name))
                {
                    yield return name;
                }
            }
        }

        public static IEnumerable<string> GetDataMemberNames(this IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (TryGetColumnName(property, out string name))
                {
                    yield return name;
                }
            }
        }

        public static string GetDataMemeberName(this PropertyInfo property)
        {
            var dataMemberAttribute = property.GetCustomAttribute<DataMemberAttribute>();
            if (dataMemberAttribute != null)
            {
                return dataMemberAttribute.Name;
            }
            return string.Empty;
        }

        public static bool IsRequiredDataMember(this CustomAttributeData attributeData)
        {
            return attributeData.NamedArguments.Any(arg => arg.MemberName.Equals(Constants.IsRequired) && !(bool)arg.TypedValue.Value)
                ? false
                : true;
        }

        internal static IEnumerable<KeyValuePair<string, object>> GetColumns(object obj)
        {
            var type = obj.GetType();
            PropertyInfo[] propertyInfo = type.GetProperties(Constants.InstantPublic);
            foreach (var property in propertyInfo)
            {
                var attrData = property.CustomAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(DataMemberAttribute));
                if (attrData != null)
                {
                    if (!attrData.NamedArguments.Any(arg => arg.MemberName == Constants.IsRequired && !(bool)arg.TypedValue.Value))
                    {
                        var name = attrData.NamedArguments.FirstOrDefault(arg => arg.MemberName == Constants.DataMemberName);
                        if (name.TypedValue.Value != null)
                        {
                            yield return new KeyValuePair<string, object>(name.TypedValue.Value.ToString(), property.GetValue(obj, new object[0]));
                        }
                        else
                        {
                            // name not found
                            yield return new KeyValuePair<string, object>(property.Name, property.GetValue(obj, new object[0]));
                        }
                    }
                }
            }
        }

        internal static string GetColumnName(PropertyInfo property)
        {
            var attrData = property.CustomAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(DataMemberAttribute));
            if (attrData != null)
            {
                if (attrData.NamedArguments.Any(arg => arg.MemberName == Constants.IsRequired && !(bool)arg.TypedValue.Value))
                {
                    return null;
                }
                var name = attrData.NamedArguments.FirstOrDefault(arg => arg.MemberName == Constants.DataMemberName);
                if (name.TypedValue.Value != null)
                {
                    return name.TypedValue.Value.ToString();
                }
                // Name not found
                return property.Name;
            }
            return null;
        }

        internal static bool TryGetColumnName(PropertyInfo property, out string columnName)
        {
            var attrData = property.CustomAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(DataMemberAttribute));
            if (attrData != null)
            {
                if(attrData.NamedArguments.Any(arg => arg.MemberName == Constants.IsRequired && !(bool)arg.TypedValue.Value))
                {
                    columnName = null;
                    return false;
                }
                var name = attrData.NamedArguments.FirstOrDefault(arg => arg.MemberName == Constants.DataMemberName).TypedValue.Value;
                if (name != null)
                {
                    columnName = name.ToString();
                    return true;
                }
                // Name not found
                columnName = property.Name;
                return true;
            }
            columnName = null;
            return false;
        }

        internal static IEnumerable<string> GetFields(this DbDataReader reader)
        {
            for (int index = 0; index < reader.FieldCount; index++)
            {
                yield return reader.GetName(index);
            }
        }

        internal static KeyValuePair<string, object> GetPrimaryKey<TElement>(TElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            PropertyInfo keyProperty = GetKeyProperty(element.GetType());
            var name = GetColumnName(keyProperty);
            var value = keyProperty.GetValue(element, new object[0]);
            if (value == null)
                throw new NullReferenceException(nameof(value));
            return new KeyValuePair<string, object>(name, value);
        }

        internal static string GetPrimaryKey(Type type)
        {
            return GetColumnName(GetKeyProperty(type));
        }

        private static PropertyInfo GetKeyProperty(Type type)
        {
            var keyProperty = type.GetProperties().FirstOrDefault(p => p.IsDefined(typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));
            if (keyProperty == null)
                throw new KeyNotFoundException("Key property not found");
            return keyProperty;
        }
    }
}
