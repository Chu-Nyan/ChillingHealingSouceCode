using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : UIBase
{
    [SerializeField] public Image ItemImage;
    [SerializeField] public Button SlotButton;
    [SerializeField] public TextMeshProUGUI ItemName;
    [SerializeField] public TextMeshProUGUI ItemStack;

    private UIManager _UIM = UIManager.Instance;
    private ItemBase _item;
    private bool _isConsumable;

    public Action<Consumable, int> OnItemConsume;
    public Action OnCancelConsume;
    public Action<ItemBase, int> OnItemResell;
    
    public void Init(ItemBase item)
    {
        _item = item;
        ItemName.text = _item.UniversalStatus.Name;
        ItemType itemType = item.UniversalStatus.Type;
        ItemImage.sprite = AssetManager.GetItemIconSprite(itemType);
        
        _isConsumable = (int)item.UniversalStatus.Type is >= 20000 and < 30000;
        ItemStack.text = item.LocalStatus.Stack.ToString();
        SlotButton.onClick.AddListener(ShowItemPopUp);
        OnDeactivation += Deactivation;
    }
    
    private void ShowItemPopUp()
    {
        var popUp = _UIM.GetUI<ItemPopUpUI>(UIName.ItemPopUpUI);
        int stack = _item.Stack;
        if (_UIM._player.Character._wearingsData.ContainsValue(_item.UniversalStatus.Type))
            stack -= 1;
        popUp.Init(_item.UniversalStatus.Type,
            _isConsumable ? ItemPopUpType.InventoryConsumables : ItemPopUpType.Inventory, stack);
        popUp.OnValueChanged = ConfirmResellingItem;
        if (_isConsumable)
            popUp.OnConsumableUsed = ConfirmUsingItem;
    }

    private void ConfirmUsingItem(int amount)
    {
        OnItemConsume?.Invoke((Consumable)_item, amount);
    }

    private void CancelUsingItem()
    {
        OnCancelConsume?.Invoke();
    }

    private void ConfirmResellingItem(int amount)
    {
        OnItemResell?.Invoke(_item, amount);
    }

    private void Deactivation(UIBase uiBase)
    {
        SlotButton.onClick.RemoveAllListeners();
        OnItemConsume = null;
        OnCancelConsume = null;
    }
}
