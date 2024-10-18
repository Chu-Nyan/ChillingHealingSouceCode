using System;
using UnityEngine;
using UnityEngine.UI;

public class CoinTradingUI : UIBase
{
    [SerializeField] private Button _threeToFourBtn;
    [SerializeField] private Button _fourToHealingCoinBtn;
    [SerializeField] private Button _missionCoinToHealingCoinBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private CoinTradingModalUI _modalUI;
    [SerializeField] private Transform _btns;
    private DimmedUI _dimmedUI;

    private Action<int> _threeToFourCallback;
    private Action<int> _fourToHealingCallback;
    private Action<int> _missionCoinToHealingCallback;

    private Func<int> _getThreeToFourCount;
    private Func<int> _getfourToHealingCoinCount;
    private Func<int> _getmissionCoinToHealingCoinCount;

    private void Awake()
    {
        _cancelBtn.onClick.AddListener(Deactivate);
    }

    public override void Activate()
    {
        if (_modalUI.gameObject.activeSelf == true)
        {
            DeactiveModalUI();
            _modalUI.ChangeDefault();
        }
        base.Activate();
        _dimmedUI = UIManager.Instance.GetUI<DimmedUI>(UIName.DimmedUI);
        _dimmedUI.SetDimmed(this);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _dimmedUI.ReturnDimmed(this);
    }

    public void Init(CoinTrading trading)
    {
        _threeToFourCallback = trading.ChangeThreeToFourScale;
        _fourToHealingCallback = trading.ChangeFourToHealingCoin;
        _missionCoinToHealingCallback = trading.ChangeMissionCoinToHealingCoinScale;

        _getThreeToFourCount = trading.GetThreeToFourCount;
        _getfourToHealingCoinCount = trading.GetFourCloverCount;
        _getmissionCoinToHealingCoinCount = trading.GetMissionCoinToHealingCoinCount;

        _threeToFourBtn.onClick.AddListener(ClickThreeToFourBtn);
        _fourToHealingCoinBtn.onClick.AddListener(ClickFourToHealingCoinBtn);
        _missionCoinToHealingCoinBtn.onClick.AddListener(ClickMissionCoinToHealingCoinBtn);

        _modalUI.Init(Deactivate, DeactiveModalUI);
    }

    private void ClickThreeToFourBtn()
    {
        ActiveModalUI();
        int count = _getThreeToFourCount();
        _modalUI.Refresh("세잎 클로버", Const.TextEmoji_ThreeClover, "네잎 클로버", Const.TextEmoji_FourClover, count, _threeToFourCallback);
        _modalUI.RefreshTradeScale(CoinTrading.ThreeToFourScale);
    }

    private void ClickFourToHealingCoinBtn()
    {
        ActiveModalUI();
        int count = _getfourToHealingCoinCount();
        _modalUI.Refresh("네잎 클로버", Const.TextEmoji_FourClover, "힐링 코인", Const.TextEmoji_HealingCoin, count, _fourToHealingCallback);
        _modalUI.RefreshTradeScale(CoinTrading.FourToHealingCoinScale);
    }

    private void ClickMissionCoinToHealingCoinBtn()
    {
        ActiveModalUI();
        int count = _getmissionCoinToHealingCoinCount();
        _modalUI.Refresh("미션 코인", Const.TextEmoji_MissionCoin, "힐링 코인", Const.TextEmoji_HealingCoin, count, _missionCoinToHealingCallback);
        _modalUI.RefreshTradeScale(CoinTrading.MissionCoinToHealingCoinScale);
    }

    private void ActiveModalUI()
    {
        _modalUI.gameObject.SetActive(true);
        _btns.gameObject.SetActive(false);
    }

    private void DeactiveModalUI()
    {
        _modalUI.gameObject.SetActive(false);
        _btns.gameObject.SetActive(true);
    }
}
