namespace ChillingHealing.AI
{
    public class Sequence<T> : BaseContollerNode<T> where T : class
    {
        public override MethodResult Evaluate(T item)
        {
            if (_nodes.Count == 0)
            {
                UnityEngine.Debug.LogError("비어 있는 노드");
                return MethodResult.Failure;
            }

            for (int i = 0; i < _nodes.Count; i++)
            {
                var result = _nodes[i].Evaluate(item);

                if (result != MethodResult.Success)
                    return result;
            }

            return MethodResult.Success;
        }
    }
}
