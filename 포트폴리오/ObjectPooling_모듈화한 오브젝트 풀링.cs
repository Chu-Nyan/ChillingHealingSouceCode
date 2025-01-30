using System.Collections.Generic;
using System;

public class ObjectPooling<T> where T : class
{
    private readonly Queue<T> _queue;
    private readonly Func<T> _generateDelegate;
    private readonly Action<T> _resetDelegate;

    public ObjectPooling(Func<T> generate, Action<T> reset = null)
    {
        _queue = new Queue<T>();
        _generateDelegate = generate;
        _resetDelegate = reset;
    }

    public T Dequeue()
    {
        if (_queue.TryDequeue(out var item))
            _resetDelegate?.Invoke(item);
        else
            item = _generateDelegate?.Invoke();

        return item;
    }

    public void Enqueue(T item)
    {
        _queue.Enqueue(item);
    }
}
