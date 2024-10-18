using System;

namespace ChillingHealing.AI
{
    public class Leaf<T> : IBTNode<T> where T : class
    {
        private Func<T, MethodResult> _func;

        public Leaf(Func<T, MethodResult> func)
        {
            _func = func;
        }

        public MethodResult Evaluate(T item)
        {
            if (_func == null)
            {
                UnityEngine.Debug.LogError("비어 있는 델리게이트");
                return MethodResult.Failure;
            }

            return _func.Invoke(item);
        }
    }
}
