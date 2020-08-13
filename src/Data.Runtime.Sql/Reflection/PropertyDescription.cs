using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace SqlDb.Data.Reflection
{
    public sealed class PropertyDescription : IPropertyDescription
    {
        public readonly PropertyInfo Property;

        private MethodInfo opImplicit;

        private TypeConverter converter;

        internal readonly System.Type m_propType;

        internal readonly string m_name;

        public readonly int Order;

        public readonly bool IsRequired;

        private TypeConverter Converter { get => converter ?? (converter = TypeDescriptor.GetConverter(m_propType)); }

        public PropertyDescription(string name, int order, bool isRequied, PropertyInfo property)
        {
            m_name = name;
            Order = order;
            IsRequired = isRequied;
            Property = property;
            m_propType = property.PropertyType;
        }

        private bool resolved;

        private PropertyDescriptions subDescription;
        internal PropertyDescriptions SubDescription
        {
            get
            {
                if (resolved == false)
                {
                    if (m_propType.IsSerializable == false && m_propType.IsDefined(typeof(DataContractAttribute), false))
                        subDescription = new PropertyDescriptions(m_propType.GetPropertyDescriptions());
                    resolved = true;
                }
                return subDescription;
            }
        }

        public string Name => m_name;

        public string TypeName => m_propType.FullName;

        private MethodInfo GetImplicitOperator(System.Type valueType)
        {
            if (opImplicit == null)
            {
                opImplicit = m_propType.GetMethod(Constants.OperatorImplicit, new[] { valueType });
            }
            return opImplicit;
        }

        public bool SetValue(object obj, object value)
        {
            switch (m_propType.FullName)
            {
                case Constants.TypeDateTime:
                case Constants.TypeString:
                    Property.SetValue(obj, value);
                    break;
                default:
                    if (value == null) return true;
                    if (m_propType.IsPrimitive)
                    {
                        Property.SetValue(obj, value);
                        return true;
                    }
                    if (m_propType.IsSerializable)
                    {
                        Property.SetValue(obj, Serialization.JsonConvert.Deserialize(value.ToString(), m_propType));
                        return true;
                    }
                    //Try Implicit cast
                    var castMethod = GetImplicitOperator(value.GetType());
                    if (castMethod != null)
                    {
                        Property.SetValue(obj, castMethod.Invoke(null, new[] { value }));
                        return true;
                    }
                    //Try Change Type
                    TypeConverter converter = Converter;
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        Property.SetValue(obj, converter.ConvertFrom(value));
                        return true;
                    }
                    return false;
            }
            return true;
        }

        public object GetValue(object obj)
        {
            return Property.GetValue(obj);
        }

        public override string ToString()
        {
            return m_name;
        }
    }
}
