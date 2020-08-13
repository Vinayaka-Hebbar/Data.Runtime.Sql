namespace SqlDb.Data
{
    public interface IPropertyDescription
    {
        string Name { get; }
        object GetValue(object obj);
        bool SetValue(object obj, object value);
        string TypeName { get; }
    }
}