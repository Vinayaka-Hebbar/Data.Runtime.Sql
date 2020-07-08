namespace SqlDb.Data
{
    /// <summary>
    /// Default Class for connection options
    /// </summary>
    public struct ConnectionOption : IConnectionOptions
    {
        public ConnectionOption(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }
    }
}
