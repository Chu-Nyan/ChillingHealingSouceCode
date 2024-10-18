using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlotUI : UIBase
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Button _slotButton;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDesc;
    [SerializeField] private TextMeshProUGUI _price;

    private ItemType _itemType;
    private ItemGenerator _itemGenerator = ItemGenerator.Instance;
    private UIManager _UIM = UIManager.Instance;

    public Action<ItemType, int> OnItemPurchase;

    public void Init(ItemType itemType)
    {
        _itemType = itemType;
        _itemImage.sprite = AssetManager.GetItemIconSprite(itemType);

        ItemUniversalStatus universalStatus = _itemGenerator.GetDB(_itemType);
        _itemName.text = universalStatus.Name;
        _itemDesc.text = universalStatus.Desc;
        _price.text = $"{Const.TextEmoji_HealingCoin} {universalStatus.Price.ToString()}";

        _slotButton.onClick.AddListener(ShowItemPopUp);
        OnDeactivation += Deactivation;
    }

    private void ShowItemPopUp()
    {
        var popUp = _UIM.GetUI<ItemPopUpUI>(UIName.ItemPopUpUI);
        int maxValue;
        if (_itemType == ItemType.CarUpgrade)
            maxValue = 1;
        else
        {
            ItemCategory category;
            switch (_itemType)
            {
                case ItemType.FurnitureBox:
                case ItemType.PropsBox:
                    category = ItemCategory.Furniture;
                    break;
                case ItemType.HairBox:
                case ItemType.ClothingBox:
                case ItemType.AccessoryBox:
                    category = ItemCategory.Equipment;
                    break;
                default:
                    category = ItemGenerator.SwitchTypeToItemCategory(_itemType);
                    break;
            }
            maxValue = _UIM._player.Inventory.Space.GetMaxSpace(category) - GetLeftSlotsInInventory(_itemType);
        }
        popUp.Init(_itemType, ItemPopUpType.Shop, maxValue);
        popUp.OnValueChanged = CheckBuyingItem;
    }

    private int GetLeftSlotsInInventory(ItemType itemType)
    {
        ItemCategory category;
        if (itemType == ItemType.HairBox || itemType == ItemType.ClothingBox || itemType == ItemType.AccessoryBox)
            category = ItemCategory.Equipment;
        else if (itemType == ItemType.FurnitureBox || itemType == ItemType.PropsBox)
            category = ItemCategory.Furniture;
        else // consumables?
            category = ItemGenerator.SwitchTypeToItemCategory(itemType);
        var items = _UIM._player.Inventory.Space.ItemList[category];
        return items.Count;
    }

    private void CheckBuyingItem(int value)
    {
        OnItemPurchase?.Invoke(_itemType, value);
        OnItemPurchase = null;
    }

    private void Deactivation(UIBase uiBase)
    {
        _slotButton.onClick.RemoveAllListeners();
        OnItemPurchase = null;
    }
}
