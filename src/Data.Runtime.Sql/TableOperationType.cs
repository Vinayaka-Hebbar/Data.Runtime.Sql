namespace SqlDb.Data
{
    public enum TableOperationType
    {
        Insert = 1,
        Delete = 2,
        Retrieve = 4,
        Update = 8,
        Create = 16,
        Batch = 32
    }
}