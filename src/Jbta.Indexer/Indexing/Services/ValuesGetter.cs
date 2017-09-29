using System.Collections.Generic;
using System.Linq;

namespace Jbta.Indexing.Indexing.Services
{
    internal class ValuesGetter<T>
    {
        private readonly NodeRetriever<T> _nodeRetriever;        
        private readonly Node<T> _rootNode;

        public ValuesGetter(
            NodeRetriever<T> nodeRetriever,
            Node<T> rootNode)
        {
            _nodeRetriever = nodeRetriever;
            _rootNode = rootNode;
        }

        public IEnumerable<T> Get(string query, SearchSettings searchSettings)
        {
            var keyNode = searchSettings.CaseSensetive 
                ? _nodeRetriever.Retrieve(_rootNode, query, 0)
                : _nodeRetriever.RetrieveIgnoreCase(_rootNode, query, 0);

            if (keyNode == null)
            {
                return Enumerable.Empty<T>();
            }

            if (searchSettings.WholeWord)
            {
                return keyNode.Key.SubstringFromBegin.Equals(query)
                    ? keyNode.Values
                    : Enumerable.Empty<T>();
            }
            return keyNode.Subtree.SelectMany(n => n.Values);
        } 
    }
}