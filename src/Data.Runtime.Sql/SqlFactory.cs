using SqlDb.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
#if NETFRAMEWORK
using SqlDb.Data.ConfigFile;
#endif

namespace SqlDb.Data
{
    /// <summary>
    /// Factory Implementation of Database
    /// </summary>
    public abstract class SqlFactory
    {
        private static IList<SqlFactory> m_factories;
        static readonly object _lock = new object();

        public abstract ISqlClient CreateSqlClient();

        /// <summary>
        /// Create A factory implementation of <typeparamref name="TConnection"/>
        /// </summary>
        /// <typeparam name="TConnection">Connection Type</typeparam>
        /// <param name="options">Connection Option</param>
        /// <returns>Factory Implementation SqlDbClient </returns>
        public static async Task<SqlClient> ConnectAsync<TConnection>(IConnectionOptions options) where TConnection : ISqlClient, new()
        {
            ISqlClient impl = new TConnection
            {
                Options = options
            };
            if (await impl.ConnectAsync())
                return new SqlClient(impl);
            throw new System.OperationCanceledException("Unable to connect to Database");
        }


        internal static IList<SqlFactory> Factories
        {
            get
            {
                lock (_lock)
                {

                    if (m_factories == null)
                    {
                        m_factories = new List<SqlFactory>();
#if NETFRAMEWORK
                        GetDefaultFactories();
#endif
                    }
                    return m_factories;
                }
            }
        }

#if NETFRAMEWORK
        public static SqlFactory Default
        {
            get
            {
                return Factories[0];
            }
        }

        private static void GetDefaultFactories()
        {
            SqlConfigSection config = SqlConfigSection.Instance;
            var defaultFactory = config.DefaultConnectionFactory;
            if (defaultFactory == null)
                throw new System.Exception("No Factory Provider");
            var type = defaultFactory.GetFactoryType();
            var factory = System.Activator.CreateInstance(type, defaultFactory.Parameters.GetTypedParameterValues());
            if (factory is SqlFactory)
            {
                m_factories.Add((SqlFactory)factory);
            }
            else
            {
                throw new System.Exception("Factory Connection type must be inherit" + typeof(SqlFactory).FullName);
            }
        }
#endif

        public static void SetFactory(SqlFactory factory)
        {
            if (factory == null)
                throw new System.ArgumentNullException("factory");
            lock (_lock)
            {
                Debug.WriteLine("SetFactory: " + factory == null ? "(null)" : factory.GetType().Name);
                InternalSetFactories(new SqlFactory[] { factory });
            }
        }

        private static void InternalSetFactories(IList<SqlFactory> factories)
        {
            m_factories = new System.Collections.ObjectModel.ReadOnlyCollection<SqlFactory>(factories);
        }
    }
}
