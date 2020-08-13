namespace SqlDb.Data
{
    public struct TableColumn
    {
        public readonly string Name;
        public readonly string Type;
        public readonly bool Nullable;
        public readonly object[] Constraints;

        public TableColumn(string name, string type, bool nullable = true, object[] constraints = null)
        {
            Name = name;
            Type = type;
            Nullable = nullable;
            Constraints = constraints;
        }
    }
}
