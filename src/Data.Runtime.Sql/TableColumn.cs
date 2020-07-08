namespace SqlDb.Data
{
    public struct TableColumn
    {
        public readonly string Name;
        public readonly string Type;
        public readonly string Constraints;

        public TableColumn(string name, string type, string constraints)
        {
            Name = name;
            Type = type;
            Constraints = constraints;
        }
    }
}
