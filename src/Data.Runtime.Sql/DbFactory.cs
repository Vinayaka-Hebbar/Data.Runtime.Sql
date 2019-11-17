using System.Threading.Tasks;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Factory Implementation of Database
    /// </summary>
    public static class DbFactory
    {
        /// <summary>
        /// Create A factory implementation of <typeparamref name="TConnection"/>
        /// </summary>
        /// <typeparam name="TConnection">Connection Type</typeparam>
        /// <param name="options">Connection Option</param>
        /// <returns>Factory Implementation SqlDbClient </returns>
        public static async Task<SqlDbClient> CreateAsync<TConnection>(DbConnectionOptions options) where TConnection : IDbClientImplementation, new()
        {
            IDbClientImplementation clientImplementation = new TConnection();
            clientImplementation.Options.Set(options);
            await clientImplementation.ConnectAsync();
            return new SqlDbClient(clientImplementation);
        }
    }
}
