using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeModalUI : UIBase
{
    private static readonly string Camping = "캠핑 아이템";
    private static readonly string NPC = "동물 친구들";

    private static readonly string Level = "lv";
    private static readonly string PerHour = "/ 시간";

    [SerializeField] private TMP_Text TitleName;
    [SerializeField] private Image[] _images;
    [SerializeField] private TMP_Text[] _names;
    [SerializeField] private TMP_Text[] _lv;
    [SerializeField] private TMP_Text[] _values;
    [SerializeField] private TMP_Text _requestItem;
    [SerializeField] private Button _upgradeBtn;
    [SerializeField] private Button _closeBtn;
    private DimmedUI _dimmedUI;

    private Action _upgradeBtnClicked;

    private void Awake()
    {
        _upgradeBtn.onClick.AddListener(OnUpgradeBtnClicked);
        _closeBtn.onClick.AddListener(() => Deactivate());
    }

    public void Init(CollectionUIData uiData, LevelData before, LevelData after, Sprite sprite, Action success)
    {
        SetTitleName(uiData.Type);
        SetSprtie(sprite);
        SetName(uiData.TitleName);
        SetLevel(before.Level);
        SetLevelValue(before.Coin, after.Coin);
        SetRequestAmount(before.CoinType, before.NextLevelPrice);
        _upgradeBtnClicked = success;
        _dimmedUI = UIManager.Instance.GetUI<DimmedUI>(UIName.DimmedUI);
        _dimmedUI.SetDimmed(this);
    }

    private void SetTitleName(UpgradeType type)
    {
        TitleName.text = type == UpgradeType.Camping ? Camping : NPC;
    }

    private void SetSprtie(Sprite sprite)
    {
        foreach (var item in _images)
        {
            item.sprite = sprite;
        }
    }

    private void SetName(string name)
    {
        foreach (var item in _names)
        {
            item.text = name;
        }
    }

    private void SetLevel(int lv)
    {
        _lv[0].text = $"{Level}. {lv}";
        _lv[1].text = $"{Level}. {lv + 1}";
    }

    private void SetLevelValue(int before, int after)
    {
        _values[0].text = $"{Const.TextEmoji_HealingCoin} {before} {PerHour}";
        _values[1].text = $"{Const.TextEmoji_HealingCoin} {after} {PerHour}";
    }

    private void SetRequestAmount(CoinType type, int requestAmount)
    {
        string imoge;
        if (type == CoinType.Mission)
            imoge = Const.TextEmoji_MissionCoin;
        else
            imoge = Const.TextEmoji_HealingCoin;

        _requestItem.text = $"{imoge} : {requestAmount}";
    }

    private void OnUpgradeBtnClicked()
    {
        _upgradeBtnClicked?.Invoke();
        Deactivate();
    }

    public override void Deactivate()
    {
        _dimmedUI.ReturnDimmed(this);
        base.Deactivate();
    }
}
