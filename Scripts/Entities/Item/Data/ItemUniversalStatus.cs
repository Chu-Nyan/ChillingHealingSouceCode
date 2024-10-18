using System;

[Serializable]
public class ItemUniversalStatus
{
    public ItemType Type;
    public string Name;
    public string Desc;
    public CoinType CoinType;
    public int Price;
    public int MaxStack = 99;
}
