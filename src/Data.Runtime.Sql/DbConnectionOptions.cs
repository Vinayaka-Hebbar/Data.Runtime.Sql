using System.Collections;
using System.Collections.Generic;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Database Option such as ConnectionString, Username etc
    /// </summary>
    public sealed class DbConnectionOptions : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> options;

        public DbConnectionOptions()
        {
            options = new Dictionary<string, string>();
        }

        public string this[string key]
        {
            get
            {
                if (options.ContainsKey(key))
                    return options[key];
                return string.Empty;
            }
            set
            {
                if (options.ContainsKey(key))
                {
                    options[key] = value;
                }
                else
                {
                    options.Add(key, value);
                }
            }
        }

        public int Count => options.Count;

        public bool IsReadOnly => true;

        public ICollection<string> Keys => options.Keys;

        public ICollection<string> Values => options.Values;

        public void Add(KeyValuePair<string, string> item)
        {
            options.Add(item.Key, item.Value);
        }

        public void Add(string key, string value)
        {
            options.Add(key, value);
        }

        public void Clear()
        {
            options.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return options.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            options.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return options.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return options.Remove(item);
        }

        public bool Remove(string key)
        {
            return options.Remove(key);
        }

        public void Set(DbConnectionOptions options)
        {
            foreach (KeyValuePair<string, string> option in options)
            {
                this.options.Add(option.Key, option.Value);
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            return TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}