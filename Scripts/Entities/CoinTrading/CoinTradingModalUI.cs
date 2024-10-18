using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinTradingModalUI : MonoBehaviour
{
    private static readonly string defaultTitle = "재화 교환";
    private static readonly string AcceptTitle = "재화 교환이 완료되었습니다";
    private static readonly string defaultAccept = "교환하기";
    private static readonly string defaultCancel = "취소하기";
    private static readonly string TradeAccept = "돌아가기";
    private static readonly string TradeCancel = "닫기";
    private static readonly int MinAmount = 0;

    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _request;
    [SerializeField] private TMP_Text _reward;
    [SerializeField] private TMP_Text _tradeAmountText;
    [SerializeField] private Button _minusBtn;
    [SerializeField] private Button _plusBtn;
    [SerializeField] private Button _acceptBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private TMP_Text _acceptText;
    [SerializeField] private TMP_Text _cancelText;
    [SerializeField] private TMP_Text _tradeRequestIcon;
    [SerializeField] private TMP_Text _tradeRewardIcon;
    [SerializeField] private TMP_Text _tradeRequestText;
    [SerializeField] private TMP_Text _tradeRewardText;

    private int _maxAmount;
    private int _tradeAmount;
    private int _scale;
    private Action<int> _acceptAction;
    private Action _cancelAction;
    private Action _changeAction;
    private Action _closeUI;

    private void Awake()
    {
        _minusBtn.onClick.AddListener(() =>
        {
            AddTradeAmount(-1);
        });

        _plusBtn.onClick.AddListener(() =>
        {
            AddTradeAmount(+1);
        });

        _acceptBtn.onClick.AddListener(() =>
        {
            OnAccepted();
        });

        _cancelBtn.onClick.AddListener(() =>
        {
            _cancelAction?.Invoke();
        });
    }

    public void Init(Action close, Action change)
    {
        _closeUI += close;
        _changeAction = change;
    }

    public void Refresh(string requestName, string requestIcon, string rewardName, string rewardIcon, int maxCount, Action<int> acceptCallback)
    {
        _request.text = $"{requestIcon} {requestName}";
        _reward.text = $"{rewardIcon} {rewardName}";
        _tradeRequestIcon.text = requestIcon;
        _tradeRewardIcon.text = rewardIcon;
        _maxAmount = maxCount;
        _acceptAction = acceptCallback;
        _cancelAction = _changeAction;
    }

    public void RefreshTradeScale(int scale)
    {
        _tradeAmount = 0;
        _tradeAmountText.text = "0";
        _tradeRequestText.text = "0";
        _tradeRewardText.text = "0";
        _scale = scale;
    }

    private void AddTradeAmount(int amount)
    {
        _tradeAmount = Math.Clamp(_tradeAmount + amount, MinAmount, _maxAmount);
        _tradeAmountText.text = _tradeAmount.ToString();
        string request;
        string reward;
        if (_scale < 0)
        {
            request = Math.Abs(_tradeAmount * _scale).ToString();
            reward = _tradeAmount.ToString();
        }
        else
        {
            request = _tradeAmount.ToString();
            reward = (_tradeAmount * _scale).ToString();
        }


        _tradeRequestText.text = request;
        _tradeRewardText.text = reward;
    }

    private void OnAccepted()
    {
        if (_tradeAmount <= 0)
            return;

        _title.text = AcceptTitle;
        _acceptText.text = TradeAccept;
        _cancelText.text = TradeCancel;
        _plusBtn.gameObject.SetActive(false);
        _minusBtn.gameObject.SetActive(false);
        _acceptAction?.Invoke(_tradeAmount);
        _acceptAction = Back;
        _cancelAction = _closeUI;

        void Back(int value)
        {
            ChangeDefault();
        }
    }

    public void ChangeDefault()
    {
        _plusBtn.gameObject.SetActive(true);
        _minusBtn.gameObject.SetActive(true);
        _title.text = defaultTitle;
        _acceptText.text = defaultAccept;
        _cancelText.text = defaultCancel;
        _changeAction?.Invoke();
    }
}