using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePackageSlot : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _firstPurchaseBonusImage;
    [SerializeField] private TMP_Text _firstPurchaseBonus;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _buyInfo;
    [SerializeField] private Button _btn;
    private int _index;

    private Action<int> _clicked;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClicked);
    }

    public void Refresh(int index, string itemName, Sprite sprite, Action<int> clicked)
    {
        _index = index;
        _itemName.text = itemName;
        _clicked = clicked;
        _itemIcon.sprite = sprite;
    }

    public void RefreshFirstPurchase(bool canFirstPurchase, string text = default)
    {
        _firstPurchaseBonusImage.gameObject.SetActive(canFirstPurchase);
        if (canFirstPurchase == true)
        {
            _firstPurchaseBonus.text = text;
        }
    }

    public void RefreshInfo(string BuyInfo)
    {
        _buyInfo.text = BuyInfo;
    }

    private void OnClicked()
    {
        _clicked?.Invoke(_index);
    }
}
