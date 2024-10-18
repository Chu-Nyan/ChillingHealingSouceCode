using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopUp : UIBase
{
    private const string _defaultText = "확인하시겠습니까?";
    
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;

    private DimmedUI _dimmedUI;
    
    private Action _confirmAction;
    private Action _cancelAction;

    private void Awake()
    {
        _confirmBtn.onClick.AddListener(Confirm);
        _cancelBtn.onClick.AddListener(Cancel);
    }

    public void Init(Action confirm, Action cancel = null, string text = _defaultText)
    {
        _confirmAction = confirm;
        _cancelAction = cancel;
        _text.text = text;

        _dimmedUI = UIManager.Instance.GetUI<DimmedUI>(UIName.DimmedUI);
        _dimmedUI.SetDimmed(this);
    }

    private void Confirm()
    {
        Deactivate();
        _confirmAction?.Invoke();
    }

    private void Cancel()
    {
        Deactivate();
        _cancelAction?.Invoke();
    }

    public override void Deactivate()
    {
        _dimmedUI.ReturnDimmed(this);
        base.Deactivate();
    }
}
