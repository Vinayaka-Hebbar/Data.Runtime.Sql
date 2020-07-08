#if NETFRAMEWORK
using System.Configuration;

namespace SqlDb.Data.ConfigFile
{
    public partial class SqlConfigSection : ConfigurationSection
    {
        public const string Name = "sqlConfig";

        static SqlConfigSection _instance;

        public static SqlConfigSection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (SqlConfigSection)ConfigurationManager.GetSection(Name);
                    //Console.WriteLine(s_instance);
                    if (_instance == null)
                    {
                        return new SqlConfigSection();
                    }
                }
                return _instance;
            }
        }

        [ConfigurationProperty("defaultSqlFactory")]
        public FactoryProviderElement DefaultConnectionFactory
        {
            get
            {
                return (FactoryProviderElement)base["defaultSqlFactory"];
            }
        }

        [ConfigurationProperty("providers")]
        public ProviderCollection Providers
        {
            get => (ProviderCollection)base["providers"];
        }
    }
}
#endif