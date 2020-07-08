#if NETFRAMEWORK
using SqlDb.Data.Common;
using System;
using System.Reflection;

namespace SqlDb.Data
{
    public class SqlDbFactory : SqlFactory
    {
        const BindingFlags CtorDefault = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

        Type providerType;
        public Type ProviderType
        {
            get
            {
                if (providerType == null)
                {
                    var provider = ConfigFile.SqlConfigSection.Instance.Providers[providerName];
                    providerType = Type.GetType(provider.Type, true);
                }
                return providerType;
            }
        }


        private readonly string providerName;
        private readonly object[] Arguments;

        public SqlDbFactory(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            var config = System.Configuration.ConfigurationManager.ConnectionStrings[name];

            providerName = config.ProviderName;
            Arguments = new object[] { new ConnectionOption(config.ConnectionString) };
        }

        public override ISqlClient CreateSqlClient()
        {
            return (ISqlClient)Activator.CreateInstance(ProviderType, CtorDefault, null, Arguments, null, null);
        }
    }
}

#endif