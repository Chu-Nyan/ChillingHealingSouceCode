using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingUI : UIBase
{
    [SerializeField] private SwitchToggle _bgmToggle;
    [SerializeField] private SwitchToggle _seToggle;

    [SerializeField] private Button _tutorialsBtn;
    [SerializeField] private Button _instagramBtn;
    [SerializeField] private Button _creditsBtn;
    [SerializeField] private Button _licenseBtn;
    [SerializeField] private Button _quitGame;
    [SerializeField] private Button _closeBtn;

    private Action _tutorialAction;
    private Action _instagramAction;
    private Action _creditsAction;
    private Action _licenseAction;
    private Action _quitAction;

    private DimmedUI _dimmed;

    private void Awake()
    {
        _tutorialsBtn.onClick.AddListener(OnTutorialsBtnClicked);
        _instagramBtn.onClick.AddListener(OnInstagramBtnClicked);
        _creditsBtn.onClick.AddListener(OnCreditsBtnClicked);
        _licenseBtn.onClick.AddListener(OnLicenseBtnClicked);
        _quitGame.onClick.AddListener(OnQuitBtnClicked);
        _closeBtn.onClick.AddListener(Deactivate);
    }

    public override void Activate()
    {
        base.Activate();
        _dimmed = UIManager.Instance.GetUI<DimmedUI>(UIName.DimmedUI);
        _dimmed.SetDimmed(this);
    }

    public override void Deactivate()
    {
        _dimmed.ReturnDimmed(this);
        base.Deactivate();
    }

    public void InitButtonEvent(Action tutorial, Action instagram, Action credits, Action license, Action quit)
    {
        _tutorialAction = tutorial;
        _instagramAction = instagram;
        _creditsAction = credits;
        _licenseAction = license;
        _quitAction = quit;
    }

    public void InitToggleEvent(Action<bool> bgmEvent, Action<bool> seEvent)
    {
        _bgmToggle.Init(bgmEvent);
        _seToggle.Init(seEvent);
    }

    public void Refresh(bool isBGMOn, bool isSEOn)
    {
        _bgmToggle.SetToggle(isBGMOn);
        _seToggle.SetToggle(isSEOn);
    }

    private void OnTutorialsBtnClicked()
    {
        _tutorialAction?.Invoke();
    }

    private void OnInstagramBtnClicked()
    {
        _instagramAction?.Invoke();
    }

    private void OnCreditsBtnClicked()
    {
        _creditsAction?.Invoke();
    }

    private void OnLicenseBtnClicked()
    {
        _licenseAction?.Invoke();
    }

    private void OnQuitBtnClicked()
    {
        _quitAction?.Invoke();
    }
}
