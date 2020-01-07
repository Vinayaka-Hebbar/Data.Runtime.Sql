using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Reflection Extension Methods
    /// </summary>
    public static class ReflectionHelper
    {
        public static IEnumerable<string> GetDataMemberNames(this Type type)
        {
            IEnumerable<PropertyInfo> properties = type.GetProperties(Constants.InstantPublic).GetPropertiesWithAttribute(typeof(DataMemberAttribute));
            return properties.SelectIf(GetColumnName, name => name != null);
        }

        public static IEnumerable<string> GetDataMemberNames(this IEnumerable<PropertyInfo> properties)
        {
            return properties.SelectIf(GetColumnName, name => name != null);
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

        internal static IEnumerable<string> GetFields(this DbDataReader reader)
        {
            for (int index = 0; index < reader.FieldCount; index++)
            {
                yield return reader.GetName(index);
            }
        }
    }
}
