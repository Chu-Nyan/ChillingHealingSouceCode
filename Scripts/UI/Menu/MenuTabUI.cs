using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabUI : UIBase
{
    [SerializeField] public Image TabImage;
    [SerializeField] public TextMeshProUGUI TabText;

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            TabImage.color = value ? Color.yellow : Color.white;
            _isSelected = value;
        }
    }

    public Action<MenuTabUI> OnSelected;

    public void Init(string text)
    {
        TabText.text = text;
        IsSelected = false;
        OnDeactivation += Deactivation;
    }

    public void OpenTab()
    {
        IsSelected = true;
        OnSelected?.Invoke(gameObject.GetComponent<MenuTabUI>());
    }

    private void Deactivation(UIBase uiBase)
    {
        OnSelected = null;
    }
}
