using System;
using System.Collections.Generic;

public class Inventory
{
    private static Dictionary<int, InventoryLevelData> LevelData;
    private static readonly List<ItemBase> _findHelper = new();
    private InventorySpace _space;

    public InventorySpace Space
    {
        get => _space;
    }

    public void Init(int level)
    {
        if (LevelData == null)
            LoadLevelData();

        _space = new();
        _space.Data = LevelData[level];

        if (SaveManager.Instance.IsSaveFileLoaded)
        {
            foreach (var list in SaveManager.Instance._saveData.PlayerSaveData.ItemList)
            {
                foreach (var itemData in list.Value)
                {
                    ItemBase newItem = ItemGenerator.Instance.Ready(itemData.Type).SetStack(itemData.Stack)
                        .SetDestoryedEvent(Remove).Generate();
                    newItem.LocalStatus.Index = itemData.Index;
                    _space.ItemList[list.Key].Add(newItem);
                }
            }
        }
    }

    public void SetLevel(int level)
    {
        if (_space == null)
            Init(level);

        _space.Data = LevelData[level];
    }

    public int TryAdd(ItemType type, int stack)
    {
        var successCount = stack;
        if (FindItemsNonAlloc(type).Count != 0)
        {
            foreach (var item in _findHelper)
            {
                stack = item.TryStack(stack);
                if (stack == 0)
                    return stack;
            }
        }

        ItemCategory category = ItemGenerator.SwitchTypeToItemCategory(type);
        if (_space.ItemList[category].Count < _space.GetMaxSpace(category))
        {
            var copy = ItemGenerator.Instance
                .Ready(type)
                .SetStack(stack)
                .SetDestoryedEvent(Remove)
                .Generate();

            InsertItem(category, copy);
            stack = 0;
        }
        return stack;
    }

    public void Sort(ItemType type)
    {
        var items = FindItemsNonAlloc(type);
        if (items.Count < 2)
            return;

        int maxStack = items[0].UniversalStatus.MaxStack;
        int index = 0;
        int lastIndex = items.Count - 1;
        while (index >= lastIndex)
        {
            if (items[index].Stack == maxStack)
            {
                index++;
            }
            else if (ItemBase.TryStack(items[index], items[lastIndex]) <= 0)
            {
                lastIndex++;
            }
        }
    }

    private void Remove(ItemBase item)
    {
        ItemCategory category = ItemGenerator.SwitchTypeToItemCategory(item.UniversalStatus.Type);
        _space.ItemList[category].Remove(item);
    }

    public int Remove(ItemType type, int amount)// retur
    {
        var list = FindItemsNonAlloc(type);
        var count = 0;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            int realityAmount = Math.Min(list[i].Stack, amount);
            list[i].Stack -= realityAmount;
            amount -= realityAmount;
            count += realityAmount;
        }
        var category = ItemGenerator.SwitchTypeToItemCategory(type);
        return count;
    }

    public List<ItemBase> FindItemsNonAlloc(ItemType type)
    {
        ItemCategory category = ItemGenerator.SwitchTypeToItemCategory(type);
        var categoryInven = _space.ItemList[category];
        _findHelper.Clear();

        foreach (ItemBase find in categoryInven)
        {
            if (find.UniversalStatus.Type != type)
                continue;

            _findHelper.Add(find);
        }

        return _findHelper;
    }

    public int GetTotalAmount(ItemType type)
    {
        ItemCategory category = ItemGenerator.SwitchTypeToItemCategory(type);
        var categoryInven = _space.ItemList[category];
        int sum = 0;

        foreach (ItemBase find in categoryInven)
        {
            if (find.UniversalStatus.Type != type)
                continue;

            sum += find.LocalStatus.Stack;
        }
        return sum;
    }

    private void InsertItem(ItemCategory category, ItemBase item)
    {
        var list = _space.ItemList[category];
        bool isSucess = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (item.UniversalStatus.Type < list[i].UniversalStatus.Type)
            {
                list.Insert(i, item);
                isSucess = true;
                break;
            }
        }

        if (isSucess == false)
        {
            list.Add(item);
        }
    }

    private void LoadLevelData()
    {
        LevelData = AssetManager.Instance.DeserializeJsonSync<Dictionary<int, InventoryLevelData>>(Const.Json_InventoryLevelDB);
    }
}
