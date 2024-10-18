using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HousingUI : UIBase
{
    private static readonly int TapCount = 2;
    private static readonly string[] TapName = new string[] { "가구", "소품" };

    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _saveBtn;
    [SerializeField] private TextBtn[] _taps;
    [SerializeField] private Transform _slotRoot;
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;
    [SerializeField] private Button _blueprintCancelBtn;
    [SerializeField] private SpriteBtn _original;

    private List<ItemBase> _items;
    private List<SpriteBtn> _slots;
    private List<List<Furniture>> _tapItemList;
    private int _tapIndex;

    private Action<Furniture> _itemClicked;
    private Action _uiCanceled;
    private Action _saved;
    private Action _bluePrintCanceled;
    private Action<int> _rotated;

    private void Awake()
    {
        _slots = new() { };
        _tapItemList = new();
        for (int i = 0; i < TapCount; i++)
        {
            _tapItemList.Add(new());
            _taps[i].Init(i, TapName[i], ChangeSlot);
        }
        _cancelBtn.onClick.AddListener(() => _uiCanceled?.Invoke());
        _saveBtn.onClick.AddListener(() =>
        {
            _saved?.Invoke();
            Refresh();
        });
        _leftBtn.onClick.AddListener(() => _rotated?.Invoke(-1));
        _rightBtn.onClick.AddListener(() => _rotated?.Invoke(+1));
        _blueprintCancelBtn.onClick.AddListener(() =>
        {
            _bluePrintCanceled?.Invoke();
            Refresh();
        });
    }

    public void Init(List<ItemBase> item, Action<Furniture> itemClicked, Action uiCancel, Action save)
    {
        _items = item;
        _itemClicked = itemClicked;
        _uiCanceled = uiCancel;
        _saved = save;
    }

    public void InitSelectObjectBtn(Action<int> rotateAction, Action cancelBlueprint)
    {
        _rotated += rotateAction;
        _bluePrintCanceled += cancelBlueprint;
    }

    public override void Activate()
    {
        base.Activate();
        if (_items != null)
        {
            Sort(_items);
        }
        ChangeSlot(0);
    }

    public void Refresh()
    {
        Sort(_items);
        RefreshSlot(_tapItemList[_tapIndex]);
    }

    private void ChangeSlot(int index)
    {
        _tapIndex = index;
        RefreshSlot(_tapItemList[index]);
    }

    private void RefreshSlot(List<Furniture> itemList)
    {
        while (_slots.Count <= itemList.Count)
        {
            _slots.Add(GeneratorSlot());
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            if (i >= itemList.Count)
            {
                _slots[i].gameObject.SetActive(false);
            }
            else
            {
                _slots[i].gameObject.SetActive(true);
                var sprite = AssetManager.GetItemIconSprite(itemList[i].FurnitureData.Type);
                _slots[i].SetSlotData(i, sprite);
            }
        }
    }

    private void Sort(List<ItemBase> list)
    {
        _tapItemList[0].Clear();
        _tapItemList[1].Clear();

        foreach (var item in list)
        {
            if (item is Furniture furniture == false)
                return;

            if ((int)furniture.FurnitureData.Type < 31000)
                _tapItemList[0].Add(furniture);
            else
                _tapItemList[1].Add(furniture);
        }
    }

    private SpriteBtn GeneratorSlot()
    {
        var slot = Instantiate(_original, _slotRoot);
        slot.Init(GetClickSlotNumber);
        slot.gameObject.SetActive(true);
        return slot;

        void GetClickSlotNumber(int num)
        {
            _itemClicked?.Invoke(_tapItemList[_tapIndex][num]);
        }
    }

    public void SwitchRotateButton(bool isOn)
    {
        _leftBtn.gameObject.SetActive(isOn);
        _rightBtn.gameObject.SetActive(isOn);
    }
}
