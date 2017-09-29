namespace Jbta.Indexing.Indexing.Services
{
    internal class KeyRemover<T>
    {
        private readonly KeysZipper _keysZipper;
        private readonly NodeRetriever<T> _nodeRetriever;
        private readonly Node<T> _rootNode;

        public KeyRemover(
            KeysZipper keysZipper,
            NodeRetriever<T> nodeRetriever,
            Node<T> rootNode)
        {
            _keysZipper = keysZipper;
            _nodeRetriever = nodeRetriever;
            _rootNode = rootNode;
        }

        public void Remove(string key)
        {
        }

        private void RemoveNode()
        {
            
        }

        private void MergeWithAloneChild()
        {
            
        }
    }
}