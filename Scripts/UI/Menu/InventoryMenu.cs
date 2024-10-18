using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MenuBase
{
    #region defaultValue

    private static readonly string _title = "캠핑 박스";
    private static readonly Vector2 _gridCellSize = new(300, 390);
    private static readonly TextAnchor _anchor = TextAnchor.UpperLeft;
    private static readonly Vector2 _spacing = new(10, 10);

    public enum InventoryTab
    {
        Consumable, Clothes, Furniture, Resources
    }

    public static readonly Dictionary<InventoryTab, string> TabName = new()
    {
        { InventoryTab.Consumable, "소모품" },
        { InventoryTab.Clothes, "의상" },
        { InventoryTab.Furniture, "가구" },
        { InventoryTab.Resources, "재료" }
    };
    #endregion

    private Player _player;
    private Inventory _inventory;

    private void Awake()
    {
        _player = _UIM._player;
        _inventory = _player.Inventory;
    }

    public override void Init()
    {
        Title = _title;
        Tabs = new[]
            { TabName[InventoryTab.Clothes], TabName[InventoryTab.Consumable], TabName[InventoryTab.Furniture], TabName[InventoryTab.Resources] };
        GridCellSize = _gridCellSize;
        Anchor = _anchor;
        Spacing = _spacing;
    }

    public override void Refresh(GameObject slotParent, int currentTab)
    {
        int currentCount = 0;

        foreach (ItemBase item in _inventory.Space.ItemList[(ItemCategory)currentTab])
        {
            var slot = _UIM.GetPooledUI<InventorySlotUI>(PooledUIName.InventorySlotUI);
            slot.Init(item);
            slot.transform.SetParent(slotParent.gameObject.transform, false);
            slot.transform.SetAsLastSibling();
            slot.OnItemConsume = UseItem;
            slot.OnItemResell = ResellItem;
            Slots.Add(slot);
            currentCount++;
        }

        InvenCapacity = new[] { _inventory.Space.GetCurrentSpace((ItemCategory)currentTab), _inventory.Space.GetMaxSpace((ItemCategory)currentTab) };
    }

    private void UseItem(Consumable item, int amount)
    {
        int after = _player.UseItem(item, amount * item.Effect.UsageAmount);
        OnTabChange?.Invoke(null);
    }

    private void ResellItem(ItemBase item, int amount)
    {
        if (amount == 0)
            return;
        _UIM._player.RemoveItem(item, amount);

        int earnedCoins = 0;
        ItemUniversalStatus info = item.UniversalStatus;
        switch (ItemGenerator.SwitchTypeToItemCategory(info.Type))
        {
            case ItemCategory.Equipment:
            case ItemCategory.Furniture:
                earnedCoins = 30;
                break;
            case ItemCategory.Consumable:
                earnedCoins = (int)((float)info.Price * 0.1);
                break;
        }

        _UIM._player.Data.HealingCoin += earnedCoins * amount;
        Debug.Log($"Resell earn {earnedCoins * amount}");
        OnTabChange?.Invoke(null);
    }
}
