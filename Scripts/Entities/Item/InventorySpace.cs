using System.Collections.Generic;
using Unity.Mathematics;

public class InventorySpace
{
    public const int MaxSpace = 999;

    private readonly Dictionary<ItemCategory, List<ItemBase>> _itemList;
    private InventoryLevelData _data;
    private int _bonusSpace;

    public int BonusSpace
    {
        get => _bonusSpace;
    }

    public Dictionary<ItemCategory, List<ItemBase>> ItemList
    {
        get => _itemList;
    }

    public InventoryLevelData Data
    {
        get => _data;
        set => _data = value;
    }

    public InventorySpace()
    {
        _itemList = new()
        {
            {ItemCategory.Equipment, new List<ItemBase>(32)},
            {ItemCategory.Consumable, new List<ItemBase>(32)},
            {ItemCategory.Furniture, new List<ItemBase>(32)},
            {ItemCategory.Resources, new List<ItemBase>(32)}
        };
    }

    public void AddBounsSpace(int amount)
    {
        _bonusSpace += amount;
    }

    public int GetCurrentSpace(ItemCategory category)
    {
        if (category == ItemCategory.Consumable || category == ItemCategory.Resources)
        {
            int count = 0;
            foreach (var item in ItemList[category])
            {
                count += item.Stack;
            }
            return count;
        }
        else
        {
            return ItemList[category].Count;
        }
    }

    public int GetMinCategorySpace
    {
        get
        {
            int min = int.MaxValue;
            foreach (var item in _itemList)
            {
                var space = GetMaxSpace(item.Key);
                min = math.min(min, space);
            }

            return min;
        }
    }


    public int GetMaxSpace(ItemCategory itemCategory)
    {
        return itemCategory switch
        {
            ItemCategory.Equipment => math.min(MaxSpace, _data.EquipmentSpace + _bonusSpace),
            ItemCategory.Consumable => math.min(MaxSpace, _data.ConsumableSpace + _bonusSpace),
            ItemCategory.Furniture => math.min(MaxSpace, _data.FurnitureSpace + _bonusSpace),
            _ => math.min(MaxSpace, _data.ResourcesSpace + _bonusSpace)
        };
    }
}
