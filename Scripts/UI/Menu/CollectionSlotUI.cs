using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionSlotUI : UIBase
{
    [SerializeField]
    private Image _targetImage;
    [SerializeField]
    private TMP_Text _titleName;
    [SerializeField]
    private TMP_Text _level;
    [SerializeField]
    private Button _upgradeBtn;
    [SerializeField]
    private TMP_Text _coinPerHour;
    [SerializeField]
    private TMP_Text _upgradeText;
    [SerializeField]
    private Button _infoBtn;
    [SerializeField]
    private Image _unlockImage;

    private bool _canUpgrade;
    private IUpgradeable _target;
    private Action _upgraded;

    public void Awake()
    {
        _upgradeBtn.onClick.AddListener(() => ShowUpgradeModelUI());
        _infoBtn.onClick.AddListener(() => ShowInfoUI());
        _targetImage.preserveAspect = true;
    }

    public void Refresh()
    {
        var uiData = _target.CollectionUIData;
        _target.TryGetLevelValue(uiData.Level, out var currentLevelData);
        _level.text = $"{uiData.SkillName}{currentLevelData.Level}";
        _coinPerHour.text = $"{Const.TextEmoji_HealingCoin} {currentLevelData.Coin} / 시간";
        _canUpgrade = _target.IsUpgradeable;
        if (_canUpgrade == true)
        {
            _upgradeText.text = uiData.UpgradeText;
        }
        else
        {
            _upgradeText.text = "완료";
        }
    }

    public void SetTarget(IUpgradeable target)
    {
        _target = target;
        var data = target.CollectionUIData;
        _titleName.text = data.TitleName;
        SetInfoEvent(data);
        Refresh();

    }

    public void SetTargetImage(Sprite sprite)
    {
        _targetImage.sprite = sprite;
    }

    public void SetUpgradeEvent(Action action)
    {
        _upgraded = action;
    }

    private void SetInfoEvent(CollectionUIData data)
    {
        if (data.Type == UpgradeType.Camping)
            _infoBtn.gameObject.SetActive(false);
        else
            _infoBtn.gameObject.SetActive(true);
    }

    public void SetUnlockImage(bool isUnlock)
    {
        _unlockImage.gameObject.SetActive(isUnlock == false);
    }

    private void ShowInfoUI()
    {
        var ui = UIManager.Instance.GetUI<NPCInfoUI>(UIName.NPCInfoUI);
        var data = _target.CollectionUIData;
        ui.Init(data.TitleName, _targetImage.sprite, data.Desc);
    }

    private void ShowUpgradeModelUI()
    {
        if (_canUpgrade == true)
        {
            var ui = UIManager.Instance.GetUI<UpgradeModalUI>(UIName.UpgradeModalUI);
            var currentInfo = _target.GetCurrentLevelData();
            _target.TryGetLevelValue(currentInfo.Level + 1, out var nextInfo);
            if (nextInfo == null)
                nextInfo = _target.GetCurrentLevelData();

            ui.Init(_target.CollectionUIData, currentInfo, nextInfo, _targetImage.sprite, OnUpgraded);
        }
    }

    private void OnUpgraded()
    {
        _upgraded?.Invoke();
        Refresh();
        SoundManager.Instance.PlaySE(SoundType.Upgrade);
    }
}
