using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using static Data.Runtime.Sql.Constants;

namespace Data.Runtime.Sql
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

        internal static IEnumerable<Utils.PropertyDescription> GetPropertyDescriptions(this Type type)
        {
            foreach (var property in type.GetProperties()
                .Where(p => p.IsDefined(typeof(DataMemberAttribute), false)))
            {
                var propertyType = property.PropertyType;
                var attribute = (DataMemberAttribute)property.GetCustomAttribute(typeof(DataMemberAttribute));
                var descriptor = new Utils.PropertyDescription(attribute.Name ?? property.Name, attribute.Order, attribute.IsRequired, property);
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

        internal static IEnumerable<Utils.PropertyItem> GetPropertyItems(this Type type)
        {
            foreach (var property in type.GetProperties()
                .Where(p => p.IsDefined(typeof(DataMemberAttribute), false)))
            {
                var attribute = (DataMemberAttribute)property.GetCustomAttribute(typeof(DataMemberAttribute));
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

        internal static object TryGetValue(this PropertyInfo property, object obj, Type type)
        {
            var value = property.GetValue(obj);
            if (type.IsEnum)
                return type.GetField(EnumValue, InstantPublic).GetValue(value);
            return value;
        }

        internal static IEnumerable<DbParameter> Latest(this IEnumerable<DbParameter> parameters)
        {
            List<DbParameter> result = new List<DbParameter>();
            foreach (var item in parameters)
            {
                if (result.Any(p => p.ParameterName == item.ParameterName))
                {
                    var index = result.IndexOf(p => p.ParameterName == item.ParameterName && !p.Value.Equals(item.Value));
                    if (index != -1)
                    {
                        result[index] = item;
                    }
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }

        internal static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> prediction)
        {
            int i;
            var iterator = source.GetEnumerator();
            TSource current;
            for (i = 0; iterator.MoveNext(); i++)
            {
                current = iterator.Current;
                if (prediction(current))
                    return i;
            }
            return -1;
        }
    }
}
