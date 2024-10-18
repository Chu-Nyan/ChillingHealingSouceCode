namespace ChillingHealing.AI
{
    public class BehaviorAI<T> where T : class
    {
        private BaseContollerNode<T> _root;
        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }

        public void Init(BaseContollerNode<T> root)
        {
            _root = root;
        }

        public void AddRootChild(IBTNode<T> node)
        {
            _root.AddNode(node);
        }

        public void Execute(T item)
        {
            if (_isRunning == false)
                return;

            _root.Evaluate(item);
        }
    }
}
