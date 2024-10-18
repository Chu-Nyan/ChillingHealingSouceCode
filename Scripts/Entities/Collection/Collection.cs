using System;

[Serializable]
public class Collection<T> where T : Enum
{
    public T Item;
    public bool IsUnlocked;

    public Collection(T item, bool isUnlocked)
    {
        Item = item;
        IsUnlocked = isUnlocked;
    }

    public void Unlock(bool value)
    {
        IsUnlocked = value;
    }
}
