namespace SqlDb.Data.Queries
{
    public
#if LATEST_VS
        readonly
#endif
        struct QueryString
    {
        public readonly string Value;

        public readonly TableOperationType Type;

        public QueryString(string value, TableOperationType type)
        {
            Value = value;
            Type = type;
        }
    }
}
