using System;
using System.Collections.Generic;

public class CollectionStorage<T> where T : Enum
{
    private List<Collection<T>> _collection;

    public Collection<T> this[int index] 
    { 
        get => _collection[index]; 
    }

    public int Count 
    { 
        get => _collection.Count;
    }

    public CollectionStorage()
    {
        _collection = new();
    }

    public void Add(Collection<T> item)
    {
        _collection.Add(item);
    }

    public void Unlock(T type)
    {
        for (int i = 0; i < _collection.Count; i++)
        {
            if (_collection[i].Item.Equals(type) == false)
                continue;

            SortUnlockOrder(i);
            break;
        }
    }

    private void SortUnlockOrder(int unlockIndex)
    {
        for (int i = 0; i < _collection.Count; i++)
        {
            if (_collection[i].IsUnlocked == true)
                continue;

            _collection[unlockIndex].Unlock(true);
            var temp = _collection[i];
            _collection[i] = _collection[unlockIndex];
            _collection[unlockIndex] = temp;
            break;
        }
    }
}
