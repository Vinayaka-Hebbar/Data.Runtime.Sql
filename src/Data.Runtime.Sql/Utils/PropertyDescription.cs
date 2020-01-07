using System.ComponentModel;
using System.Reflection;

namespace Data.Runtime.Sql.Utils
{
    public sealed class PropertyDescription
    {
        private readonly PropertyInfo property;

        private MethodInfo opImplicit;

        private TypeConverter converter;

        public readonly System.Type PropertyType;

        public readonly string Name;

        public readonly int Order;

        public readonly bool IsRequired;

        private TypeConverter Converter { get => converter ?? (converter = TypeDescriptor.GetConverter(PropertyType)); }

        public PropertyDescription(string name, int order, bool isRequied, PropertyInfo property)
        {
            Name = name;
            Order = order;
            IsRequired = isRequied;
            this.property = property;
            PropertyType = property.PropertyType;
        }

        internal PropertyDescriptions SubDescription { get; set; }

        private MethodInfo GetImplicitOperator(System.Type valueType)
        {
            if (opImplicit == null)
            {
                opImplicit = PropertyType.GetMethod(Constants.OperatorImplicit, new[] { valueType });
            }
            return opImplicit;
        }

        internal  bool TrySetValue(string name, object obj, object value)
        {
            switch (PropertyType.FullName)
            {
                case Constants.TypeStringArray:
                    string stringValue = value.ToString();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        property.SetValue(obj, System.Linq.Enumerable.Empty<string>());
                    }
                    property.SetValue(obj, stringValue.Split(Constants.CommaChar));
                    break;
                case Constants.TypeDateTime:
                case Constants.TypeString:
                    property.SetValue(obj, value);
                    break;
                default:
                    if (value == null) return true;
                    if (PropertyType.IsValueType)
                    {
                        property.SetValue(obj, value);
                        return true;
                    }
                    if (PropertyType.IsSerializable)
                    {
                        property.SetValue(obj, Json.Serialization.JsonConvert.Deserialize(value.ToString(), PropertyType));
                        return true;
                    }
                    if (SubDescription != null)
                    {
                        var instance = property.GetValue(obj);
                        if (instance == null)
                        {
                            instance = System.Activator.CreateInstance(PropertyType);
                            property.SetValue(obj, instance);
                        }
                        return SubDescription.TrySetValue(name, instance, value);
                    }
                    //Try Implicit cast
                    var castMethod = GetImplicitOperator(value.GetType());
                    if (castMethod != null)
                    {
                        property.SetValue(obj, castMethod.Invoke(null, new[] { value }));
                        return true;
                    }
                    //Try Change Type
                    TypeConverter converter = Converter;
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        property.SetValue(obj, converter.ConvertFrom(value));
                        return true;
                    }
                    return false;
            }
            return true;
        }

        public object GetValue(object obj)
        {
            return property.GetValue(obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
