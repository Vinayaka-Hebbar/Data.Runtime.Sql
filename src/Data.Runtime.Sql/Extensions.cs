using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SqlDb.Data
{
    /// <summary>
    /// Extension Methods
    /// </summary>
    internal static class Extensions
    {
        internal static PropertyInfo GetPropertyWithAttribute(this IEnumerable<PropertyInfo> properties, Func<CustomAttributeData, bool> condition)
        {
            return properties.FirstOrDefault(property => property.CustomAttributes.Any(condition));
        }

        internal static IEnumerable<PropertyInfo> GetPropertiesWithAttribute(this IEnumerable<PropertyInfo> properties, Type attributeType)
        {
            return properties.Where(property => property.IsDefined(attributeType, false));
        }

        static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            var attributes = member.GetCustomAttributes(typeof(TAttribute), false);
            if (attributes.Length == 0)
                return null;
            return (TAttribute)attributes[0];
        }

        internal static IEnumerable<Reflection.PropertyDescription> GetPropertyDescriptions(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                var attr = property.GetCustomAttribute<DataMemberAttribute>();
                if (attr != null)
                {
                    yield return new Reflection.PropertyDescription(attr.Name ?? property.Name, attr.Order, attr.IsRequired, property);
                }
            }
        }

        internal static IEnumerable<Utils.PropertyItem> GetPropertyItems(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                var attribute = property.GetCustomAttribute<DataMemberAttribute>();
                if (attribute != null)
                    yield return new Utils.PropertyItem(property, attribute.Name ?? property.Name, attribute.Order, attribute.IsRequired);
            }
        }
    }
}
