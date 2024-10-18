namespace ChillingHealing.AI
{
    public interface IBTNode<T> where T : class
    {
        public MethodResult Evaluate(T item);
    }
}
