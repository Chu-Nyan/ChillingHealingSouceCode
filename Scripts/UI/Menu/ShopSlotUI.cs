using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _categoryText;
    [SerializeField] private GridLayoutGroup _slotParent;

    private List<ShopItemSlotUI> _slots = new();
    private UIManager _UIM = UIManager.Instance;

    public Action<ItemType, int> OnItemPurchase;
    
    public void Init(string categoryText, List<ItemType> items)
    {
        _categoryText.text = categoryText;
        _slotParent.cellSize = new(290, 430);
        _slotParent.spacing = new(15, 15);
        foreach (var item in items)
        {
            var slot = _UIM.GetPooledUI<ShopItemSlotUI>(PooledUIName.ShopItemSlotUI);
            slot.Init(item);
            slot.transform.SetParent(_slotParent.gameObject.transform, false);
            slot.transform.SetAsLastSibling();
            slot.OnItemPurchase = BuyItem;
            _slots.Add(slot);
        }

        OnDeactivation += Deactivation;
    }

    private void BuyItem(ItemType type, int value)
    {
        OnItemPurchase?.Invoke(type, value);
    }

    private void Deactivation(UIBase uiBase)
    {
        foreach (var slot in _slots)
        {
            slot.Deactivate();
        }
        _slots.Clear();
        OnItemPurchase = null;
    }
}
