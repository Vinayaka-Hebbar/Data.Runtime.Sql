

using System.Threading.Tasks;
/**
* @author Vinayaka Hebbar
**/
namespace SqlDb.Data.Queries
{
    public abstract class TableFilterQuery : TableQueryBase
    {
        protected TableFilterQuery(SqlTable table) : base(table)
        {

        }

        public string WhereFilter { get; set; }

        public abstract Task<QueryResult> ExecuteAsync();
    }
}
