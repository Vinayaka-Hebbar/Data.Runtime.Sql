namespace SqlDb.Data.Constraints
{
    public struct DefaultValue
    {
        public readonly object Value;

        public DefaultValue(object value)
        {
            Value = value;
        }
    }
}
