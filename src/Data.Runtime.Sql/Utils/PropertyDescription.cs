using System;
using System.ComponentModel;
using System.Reflection;
using static Data.Runtime.Sql.Constants;

namespace Data.Runtime.Sql.Utils
{
    public sealed class PropertyDescription
    {
        private readonly PropertyInfo property;

        public readonly Type Type;

        public readonly string Name;

        public readonly int Order;

        private MethodInfo opImplicit;

        private TypeConverter converter;

        public TypeConverter Converter { get => converter ?? (converter = TypeDescriptor.GetConverter(Type)); }

        public PropertyDescription(string name,int order, PropertyInfo property)
        {
            Name = name;
            Order = order;
            this.property = property;
            Type = property.PropertyType;
        }

        private MethodInfo GetImplicitOperator(Type valueType)
        {
            if (opImplicit == null)
            {
                opImplicit = Type.GetMethod(OperatorImplicit, new[] { valueType });
            }
            return opImplicit;
        }

        internal void SetValue(object obj, object value)
        {
            switch (Type.FullName)
            {
                case TypeStringArray:
                    string stringValue = value.ToString();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        property.SetValue(obj, System.Linq.Enumerable.Empty<string>());
                        return;
                    }
                    property.SetValue(obj, stringValue.Split(CommaChar));
                    return;
                case TypeDateTime:
                case TypeString:
                    property.SetValue(obj, value);
                    return;
                default:
                    if (value == null) return;
                    if (Type.IsValueType)
                    {
                        property.SetValue(obj, value);
                        return;
                    }
                    if (Type.IsSerializable)
                    {
                        property.SetValue(obj, Json.Serialization.JsonConvert.Deserialize(value.ToString(), Type));
                        return;
                    }
                    //Try Implicit cast
                    var castMethod = GetImplicitOperator(value.GetType());
                    if (castMethod != null)
                    {
                        property.SetValue(obj, castMethod.Invoke(null, new[] { value }));
                        return;
                    }
                    //Try Change Type
                    TypeConverter converter = Converter;
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        property.SetValue(obj, converter.ConvertFrom(value));
                    }
                    return;
            }
        }

        public object GetValue(object obj)
        {
            return property.GetValue(obj);
        }
    }
}
