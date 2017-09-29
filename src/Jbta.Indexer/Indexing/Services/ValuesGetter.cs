using System.Collections.Generic;
using System.Linq;

namespace Jbta.Indexing.Indexing.Services
{
    internal class ValuesGetter<T>
    {
        private readonly NodeRetriever<T> _nodeRetriever;
        private readonly Node<T> _rootNode;

        public ValuesGetter(NodeRetriever<T> nodeRetriever, Node<T> rootNode)
        {
            _nodeRetriever = nodeRetriever;
            _rootNode = rootNode;
        }

        public IEnumerable<T> Get(string query)
        {
            var keyNode = _nodeRetriever.Retrieve(_rootNode, query, 0);
            return keyNode?.Subtree.SelectMany(n => n.Values) ?? Enumerable.Empty<T>();
        } 
    }
}