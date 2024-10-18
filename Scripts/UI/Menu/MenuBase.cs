using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
    public string Title;
    public string[] Tabs;
    private Vector2 _gridCellSize;
    private TextAnchor _anchor;
    private Vector2 _spacing = new(30, 30);
    private int[] _invenCapacity = {0, 0};

    public Vector2 GridCellSize
    {
        get => _gridCellSize;
        set
        {
            _gridCellSize = value;
            OnCellSizeChanged?.Invoke(_gridCellSize);
        }
    }

    public TextAnchor Anchor
    {
        get => _anchor;
        set
        {
            _anchor = value;
            OnAlignmentChanged?.Invoke(_anchor);
        }
    }

    public Vector2 Spacing
    {
        get => _spacing;
        set
        {
            _spacing = value;
            OnSpacingChanged?.Invoke(_spacing);
        }
    }

    public int[] InvenCapacity
    {
        get => _invenCapacity;
        set
        {
            _invenCapacity = value;
            OnInvenCapacityChanged?.Invoke(_invenCapacity);
        }
    }
    
    public List<UIBase> Slots = new();

    protected UIManager _UIM = UIManager.Instance;

    public Action<MenuTabUI> OnTabChange;
    public Action<Vector2> OnCellSizeChanged;
    public Action<TextAnchor> OnAlignmentChanged;
    public Action<Vector2> OnSpacingChanged;
    public Action<int[]> OnInvenCapacityChanged;

    public abstract void Init();
    
    public abstract void Refresh(GameObject slotParent, int currentTab);
}
