namespace SqlDb.Data.Utils
{
    public
#if LATEST_VS
        readonly
#endif
        struct PropertyItem
    {
        public readonly System.Reflection.PropertyInfo Property;

        public readonly System.Type Type;

        public readonly string Name;

        public readonly int Order;

        public readonly bool IsRequired;

        public PropertyItem(System.Reflection.PropertyInfo property, string name, int order, bool isRequired)
        {
            Property = property;
            Type = property.PropertyType;
            Name = name;
            Order = order;
            IsRequired = isRequired;
        }

        public object GetValue(object obj)
        {
            return Property.GetValue(obj);
        }
    }
}
