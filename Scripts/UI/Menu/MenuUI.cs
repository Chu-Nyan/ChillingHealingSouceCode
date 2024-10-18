using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : UIBase
{
    [SerializeField] public TextMeshProUGUI Title;
    // [SerializeField] public HorizontalLayoutGroup TabParent;
    [SerializeField] public MenuTabUI[] TabButtonList;
    [SerializeField] public GridLayoutGroup SlotParent;
    [SerializeField] public Button CloseBtn;
    [SerializeField] public RectTransform ContentRectTransform;
    [SerializeField] public Image Capacity;
    
    private Dictionary<MenuType, MenuBase> _menuDictionary;
    private UIManager _UIM = UIManager.Instance;
    // private List<MenuTabUI> _tabs = new();
    private List<UIBase> _slots = new();
    private DimmedUI _dimmed;
    
    private MenuType _type;
    private MenuBase _base;
    private string[] _tabTexts;

    public bool IsMenuUsing;
    public int CurrentTab;
    
    public Action<GameObject, int> OnRefresh;

    private void Awake()
    {
        InitMenuDictionary();
    }

    public void Init(MenuType type)
    {
        Capacity.gameObject.SetActive(type == MenuType.Inventory);
        if (IsMenuUsing)
            return;
        IsMenuUsing = true;
        _type = type;
        InitBase();
        InitDimmed();
        InitTab();
        
        CloseBtn.onClick.AddListener(Deactivate);

        OnRefresh += (_, _) => ContentRectTransform.anchoredPosition = new Vector3(0, 0, 0);
        OnRefresh?.Invoke(SlotParent.gameObject, CurrentTab);
    }
    
    private void InitBase()
    {
        _base = _menuDictionary[_type];
        _base.Init();
        Title.text = _base.Title;
        _tabTexts = _base.Tabs;
        SlotParent.cellSize = _base.GridCellSize;
        SlotParent.childAlignment = _base.Anchor;
        SlotParent.spacing = _base.Spacing;
        _base.OnCellSizeChanged += SetCellSize;
        _base.OnAlignmentChanged += SetAlignment;
        _base.OnSpacingChanged += SetSpacing;
        _base.OnInvenCapacityChanged += SetInvenCapacity;
        _base.OnTabChange += ChangeCurrentTab;
        OnRefresh += _base.Refresh;
    }

    public void SetInvenCapacity(int[] newCapacity)
    {
        var temp = Capacity.gameObject.GetComponentInChildren<InventoryCapacityUI>();
        temp.Actual.text = newCapacity[0].ToString();
        temp.Max.text = newCapacity[1].ToString();
    }

    public void SetCellSize(Vector2 vector2)
    {
        SlotParent.cellSize = vector2;
    }

    public void SetAlignment(TextAnchor anchor)
    {
        SlotParent.childAlignment = anchor;
    }

    public void SetSpacing(Vector2 vector2)
    {
        SlotParent.spacing = vector2;
    }

    private void InitDimmed()
    {
        _dimmed = _UIM.GetUI<DimmedUI>(UIName.DimmedUI);
        _dimmed.SetDimmed(this);
    }

    private void InitTab()
    {
        List<MenuTabUI> helper = new(4);
        for (int i = 0; i < _tabTexts.Length; i++)
        {
            // var tab = _UIM.GetPooledUI<MenuTabUI>(PooledUIName.MenuTabUI);
            TabButtonList[i].Init(_tabTexts[i]);
            TabButtonList[i].gameObject.SetActive(true);
            // tab.transform.SetParent(TabParent.gameObject.transform);
            // tab.transform.SetAsLastSibling();
            TabButtonList[i].OnSelected += ChangeCurrentTab;
            helper.Add(TabButtonList[i]);
        }

        for (int j = 3; j >= _tabTexts.Length; j--)
        {
            TabButtonList[j].gameObject.SetActive(false);
        }
        
        CurrentTab = 0;
        if (helper.Count != 0)
            TabButtonList[CurrentTab].IsSelected = true;
    }

    private void ChangeCurrentTab(MenuTabUI tab)
    {
        if (tab != null && _tabTexts.Length != 0)
        {
            for (int i = 0; i < _tabTexts.Length; i++)
            {
                TabButtonList[i].IsSelected = tab == TabButtonList[i];
            
                if (tab == TabButtonList[i])
                    CurrentTab = i;
            }
        }
        ClearSlots();
        OnRefresh?.Invoke(SlotParent.gameObject, CurrentTab);
        _slots = _base.Slots;
    }

    private void ClearSlots()
    {
        foreach (var prevSlot in _slots)
            prevSlot.Deactivate();
        foreach (var prevBaseSlot in _base.Slots)
            prevBaseSlot.Deactivate();
        _slots.Clear();
        _base.Slots.Clear();
    }

    public override void Deactivate()
    {
        _dimmed.ReturnDimmed(this);
        base.Deactivate();
        // ClearTabs();
        ClearSlots();
        SlotParent.childAlignment = TextAnchor.UpperLeft;
        CloseBtn.onClick.RemoveAllListeners();
        OnRefresh = null;
        IsMenuUsing = false;
    }
    
    private void InitMenuDictionary()
    {
        _menuDictionary = new Dictionary<MenuType, MenuBase>
        {
            { MenuType.Inventory, gameObject.AddComponent<InventoryMenu>() }, // AddComp 괜춘?
            { MenuType.Mission, gameObject.AddComponent<MissionMenu>() },
            { MenuType.MissionBoard, gameObject.AddComponent<MissionBoardMenu>() },
            { MenuType.NPCCollection, gameObject.AddComponent<NPCCollectionMenu>() },
            { MenuType.CampingItemCollection, gameObject.AddComponent<CampingItemCollectionMenu>() },
            { MenuType.Shop, gameObject.AddComponent<ShopMenu>() },
            { MenuType.WorldMap , gameObject.AddComponent<WorldMapMenu>()}
        };
    }
}
