namespace SqlDb.Data.Constraints
{
    public struct Null
    {
        public readonly bool Nullable;

        internal Null(bool nullable)
        {
            Nullable = nullable;
        }
    }
}
