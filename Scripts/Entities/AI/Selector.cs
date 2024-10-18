namespace ChillingHealing.AI
{
    public class Selector<T> : BaseContollerNode<T> where T : class
    {
        public override MethodResult Evaluate(T item)
        {
            if (_nodes.Count == 0)
            {
                UnityEngine.Debug.LogError("비어 있는 노드");
                return MethodResult.Failure;
            }

            foreach (var node in _nodes)
            {
                var result = node.Evaluate(item);

                if (result != MethodResult.Failure)
                    return result;
            }

            return MethodResult.Failure;
        }
    }
}
