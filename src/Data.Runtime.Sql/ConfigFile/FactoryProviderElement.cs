#if NETFRAMEWORK
using System;
using System.Configuration;

namespace SqlDb.Data.ConfigFile
{
    public sealed class FactoryProviderElement : ConfigurationElement
    {
        private const string TypeKey = "type";
        private const string ParametersKey = "parameters";

        [ConfigurationProperty(TypeKey, IsRequired = true)]
        public string FactoryTypeName
        {
            get { return (string)this[TypeKey]; }
            set { this[TypeKey] = value; }
        }

        [ConfigurationProperty(ParametersKey)]
        public ParameterCollection Parameters
        {
            get { return (ParameterCollection)base[ParametersKey]; }
        }

        public Type GetFactoryType()
        {
            return Type.GetType(FactoryTypeName, throwOnError: true);
        }

    }
}
#endif