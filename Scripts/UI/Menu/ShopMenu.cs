using System.Collections.Generic;
using UnityEngine;

public enum ShopTab
{
    Character,
    CampingCar,
    Cash,
    PaidItem
}

public class ShopMenu : MenuBase
{
    #region defaultValue
    private static readonly string _title = "힐링 상점";
    private static readonly Vector2 _gridCellSize = new(900, 560);
    private static readonly Vector2 _paidItemCellSize = new(900, 1650);
    private static readonly Vector2 TEMP_gridCellSize = new(900, 300);
    private static readonly TextAnchor _anchor = TextAnchor.UpperCenter;

    public static readonly Dictionary<ShopTab, string> TabName = new()
    {
        { ShopTab.Character, "캐릭터" },
        { ShopTab.CampingCar, "캠핑카" },
        { ShopTab.Cash, "패키지" },
        { ShopTab.PaidItem, "재화" },
    };
    #endregion

    private Player _player;
    private Shop _shop;

    private void Awake()
    {
        _player = _UIM._player;
        _shop = _UIM._shop;
    }

    public override void Init()
    {
        Title = _title;
        Tabs = new[]
        {
            TabName[ShopTab.Character],
            TabName[ShopTab.CampingCar],
            TabName[ShopTab.Cash],
            TabName[ShopTab.PaidItem]
        };
        GridCellSize = _gridCellSize;
        Anchor = _anchor;
    }

    public override void Refresh(GameObject slotParent, int currentTab)
    {
        ShopTab tab = (ShopTab)currentTab;
        switch (tab)
        {
            case ShopTab.Cash:
                GridCellSize = TEMP_gridCellSize;
                var packs = IAPManager.Instance.GetShopPackageList();
                for (int i = 0; i < packs.Count; i++)
                {
                    if (IAPManager.Instance.HasReceipt(packs[i].Type) == true && packs[i].IsConsumeable == true)
                        continue;
            
                    var slot = AddNewPackageSlot(slotParent, packs[i].Type, packs[i].SpritePath);
                    slot.transform.SetAsLastSibling();
                    Slots.Add(slot);
                }
                break;
            case ShopTab.PaidItem:
                GridCellSize = _paidItemCellSize;
                Slots.Add(AddShopPaidItemTapUI(slotParent, _shop.PaidPackages, _shop.PaidAditems));
                break;
            default:
                GridCellSize = _gridCellSize;
                ShopItemListData tabData = _shop.List[tab];
                for (int i = 0; i < tabData.Desc.Length; i++)
                {
                    if (_player.Data.CampingCarLevel == 3 && i == 0 && currentTab == (int)ShopTab.CampingCar)
                        continue;
                    Slots.Add(AddNewSlot(tabData.Desc[i], slotParent, new List<ItemType>(tabData.ItemType[i])));
                }
                break;
        }
    }

    private ShopSlotUI AddNewSlot(string categoryText, GameObject slotParent, List<ItemType> items)
    {
        var slot = _UIM.GetPooledUI<ShopSlotUI>(PooledUIName.ShopSlotUI);
        slot.Init(categoryText, items);
        slot.transform.SetParent(slotParent.gameObject.transform, false);
        slot.transform.SetAsLastSibling();
        items.Clear();
        slot.OnItemPurchase = BuyItem;
        return slot;
    }

    private TempPackageUI AddNewPackageSlot(GameObject slotParent, PackageType type, string spritePath)
    {
        var tempPackageUI = _UIM.GetPooledUI<TempPackageUI>(PooledUIName.TempPackageUI);
        tempPackageUI.transform.SetParent(slotParent.gameObject.transform, false);
        tempPackageUI.Init(type, spritePath);
        return tempPackageUI;
    }

    private ShopPaidItemTapUI AddShopPaidItemTapUI(GameObject slotParent, PackageType[] packages, DailyBuyItemData[] adPackages)
    {
        var ui = _UIM.GetPooledUI<ShopPaidItemTapUI>(PooledUIName.ShopPaidItemTapUI);
        ui.transform.SetParent(slotParent.gameObject.transform, false);
        ui.Init(_player, packages, adPackages);
        return ui;
    }

    private void BuyItem(ItemType type, int amount)
    {
        bool success = _shop.TryBuy(_player, type, amount);
        OnTabChange?.Invoke(null);
    }
}
