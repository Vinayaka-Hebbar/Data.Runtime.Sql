namespace SqlDb.Data.Queries
{
    public class TableCreateQuery<TElement> : TableQueryBase
    {
        public TableCreateQuery(SqlTable table) : base(table)
        {
        }

        public override TableOperationType OperationType => TableOperationType.Create;

        public override string GetQueryString()
        {
            return string.Empty;
        }
    }
}
