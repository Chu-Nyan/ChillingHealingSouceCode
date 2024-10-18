using System;
using UnityEngine;

public abstract class ItemBase
{
    protected ItemUniversalStatus _universalStatus;
    protected ItemLocalStatus _localStatus = new();

    public event Action<ItemBase> Destroyed;

    public ItemUniversalStatus UniversalStatus
    {
        get => _universalStatus;
    }

    public ItemLocalStatus LocalStatus
    {
        get => _localStatus;
        set => _localStatus = value;
    }

    public int Index
    {
        get => _localStatus.Index;
        set => _localStatus.Index = value;
    }

    public int Stack
    {
        get => _localStatus.Stack;
        set
        {
            if (value > 0)
                _localStatus.Stack = Mathf.Min(value, _universalStatus.MaxStack);
            else
                Destroy();
        }
    }

    public int TryStack(int amount)
    {
        if (_localStatus.Stack == _universalStatus.MaxStack)
            return amount;

        var sum = _localStatus.Stack + amount;
        var maxStack = UniversalStatus.MaxStack;
        if (sum <= maxStack)
            _localStatus.Stack = sum;
        else
            _localStatus.Stack = maxStack;

        var remain = sum - maxStack;
        return remain > 0 ? remain : 0;
    }

    public static int TryStack(ItemBase target, ItemBase item)
    {
        item.Stack = target.TryStack(item.Stack);
        return item.Stack;
    }

    public void Destroy()
    {
        Destroyed?.Invoke(this);
        Destroyed = null;
    }
}
