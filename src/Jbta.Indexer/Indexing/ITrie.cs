using System.Collections.Generic;

namespace Jbta.Indexing.Indexing
{
    public interface ITrie<T>
    {
        void Add(string key, T value);
        void Remove(string key);
        IEnumerable<T> Search(string query);
    }
}