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

        internal static IEnumerable<Utils.PropertyDescription> GetPropertyDescriptions(this IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties
                .Where(p => p.IsDefined(typeof(DataMemberAttribute))))
            {
                var attribute = property.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Equals(typeof(DataMemberAttribute)));
                if (!attribute.NamedArguments.Any(arg => arg.MemberName.Equals(IsRequired) && !(bool)arg.TypedValue.Value))
                {
                    CustomAttributeNamedArgument name = attribute.NamedArguments.FirstOrDefault(arg => arg.MemberName.Equals(DataMemberName));
                    CustomAttributeNamedArgument order = attribute.NamedArguments.FirstOrDefault(arg => arg.MemberName.Equals(Order));
                    yield return new Utils.PropertyDescription(name.MemberInfo == null ? property.Name : name.TypedValue.Value.ToString(), order.MemberInfo == null ? 0 : (int)order.TypedValue.Value, property);
                }
            }
        }

        internal static void SetElementValue(this PropertyInfo property, object element, object value)
        {
            Type type = property.PropertyType;
            switch (type.FullName)
            {
                case TypeStringArray:
                    string stringValue = value.ToString();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        property.SetValue(element, new string[0]);
                        return;
                    }
                    property.SetValue(element, stringValue.Split(CommaChar));
                    return;
                case TypeDateTime:
                case TypeString:
                    property.SetValue(element, value);
                    return;
                default:
                    if (value == null) return;
                    if (type.IsValueType)
                    {
                        property.SetValue(element, value);
                        return;
                    }
                    if (type.IsSerializable)
                        property.SetValue(element, Json.Serialization.JsonConvert.Deserialize(value.ToString(), type));
                    return;
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
