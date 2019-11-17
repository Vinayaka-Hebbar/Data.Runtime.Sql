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

        private static string GetColumnName(PropertyInfo property)
        {
            var dataMemberAttributeData = property.CustomAttributes.FirstOrDefault(attribute => attribute.AttributeType == typeof(DataMemberAttribute));
            if (dataMemberAttributeData != null)
            {
                if (dataMemberAttributeData.NamedArguments.Any(arg => arg.MemberName == Constants.IsRequired && !(bool)arg.TypedValue.Value))
                {
                    return null;
                }
                return dataMemberAttributeData.NamedArguments.FirstOrDefault(arg => arg.MemberName == Constants.DataMemberName).TypedValue.Value.ToString();
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
