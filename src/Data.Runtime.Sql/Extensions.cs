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

        internal static IEnumerable<Utils.PropertyDescription> GetPropertyDescriptions(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;
                var attr = property.GetCustomAttribute<DataMemberAttribute>();
                if (attr != null)
                {
                    var descriptor = new Utils.PropertyDescription(attr.Name ?? property.Name, attr.Order, attr.IsRequired, property);
                    if (propertyType.IsClass && propertyType.IsSerializable == false)
                    {
                        if (propertyType.IsDefined(typeof(DataContractAttribute), false))
                        {
                            descriptor.SubDescription = new Utils.PropertyDescriptions(GetPropertyDescriptions(propertyType));
                        }
                    };
                    yield return descriptor;
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

        internal static IEnumerable<TReturn> SelectIf<TSource, TReturn>(this IEnumerable<TSource> elements, Func<TSource, TReturn> select, Func<TReturn, bool> condition)
        {
            foreach (TSource element in elements)
            {
                TReturn selection = select(element);
                if (condition(selection))
                {
                    yield return selection;
                }
            }
        }

        internal static IEnumerable<DbParameter> Latest(this IEnumerable<DbParameter> parameters)
        {
            List<DbParameter> result = new List<DbParameter>();
            foreach (var item in parameters)
            {
                var index = result.FindIndex(p => string.Equals(p.ParameterName, item.ParameterName));
                // no match item found
                if (index == -1)
                {
                    result.Add(item);
                }
                else
                {
                    // continue match from here
                    index = result.FindIndex(index, p => string.Equals(p.ParameterName, item.ParameterName) && !p.Value.Equals(item.Value));
                    if (index != -1)
                    {
                        result[index] = item;
                    }
                }
            }
            return result;
        }
    }
}
